using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

/// <summary>
/// HideoutBehaviour.cs — Agrega la búsqueda en escondites al enemigo patrullador.
///
/// CÓMO ENCAJA CON EL CÓDIGO EXISTENTE:
///   - Se suscribe a DetectionEvents.OnPlayerHidden (ya existe en DetectionArc.cs)
///   - Usa el mismo AIPath que ya usa PatrolBehaviour
///   - Usa GeneralEnemyRotation igual que PatrolBehaviour
///   - NO modifica PatrolBehaviour ni ningún otro script del equipo
///
/// SETUP en Unity:
///   1. Agrega este componente al GameObject del Vagabundo.
///   2. Asigna los escondites (GameObjects con HidingInteraction) en el Inspector.
/// </summary>
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
public class HideoutBehaviour : MonoBehaviour
{
    [Header("Escondites")]
    [Tooltip("Arrastra aquí todos los GameObjects que tienen HidingInteraction (basureros, arbustos, etc.)")]
    [SerializeField] private List<Transform> hideouts;

    [Header("Búsqueda")]
    [Tooltip("Cuántos segundos inspecciona cada escondite antes de rendirse.")]
    [SerializeField] private float inspectDuration = 2.5f;

    [Tooltip("Radio para considerar que el enemigo llegó al escondite.")]
    [SerializeField] private float arrivalDistance = 0.8f;

    [Tooltip("Cuántos escondites revisa antes de volver a patrullar (0 = todos).")]
    [SerializeField] private int maxHideoutsToSearch = 2;

    // ── Componentes del equipo ────────────────────────────────────────────
    private AIPath               _aStar;
    private GeneralEnemyRotation _rotation;
    private OceanManager         _ocean;

    // ── Estado interno ────────────────────────────────────────────────────
    private bool            _isSearching = false;
    private Transform       _currentTarget;
    private float           _inspectTimer;
    private bool            _inspecting;
    private List<Transform> _searchQueue = new();
    private int             _searchedCount;

    // ─────────────────────────────────────────────────────────────────────

    void Start()
    {
        _aStar    = GetComponent<AIPath>();
        _rotation = GetComponent<GeneralEnemyRotation>();
        _ocean    = GetComponent<OceanManager>();

        // Escuchar cuando el jugador sale del cono de visión
        DetectionEvents.OnPlayerHidden += OnPlayerHidden;
    }

    void OnDestroy()
    {
        DetectionEvents.OnPlayerHidden -= OnPlayerHidden;
    }

    void Update()
    {
        if (!_isSearching) return;

        if (_currentTarget == null)
        {
            PickNextHideout();
            return;
        }

        float dist = Vector2.Distance(transform.position, _currentTarget.position);
        bool arrived = dist <= arrivalDistance || _aStar.reachedDestination;

        if (!_inspecting && arrived)
        {
            // Llegó — empezar inspección
            StartInspecting();
        }
        else if (!_inspecting)
        {
            // Caminando hacia el escondite
            _rotation.LookAt(_currentTarget.position);
        }
        else
        {
            // Inspeccionando
            _inspectTimer -= Time.deltaTime;

            // Mirar a los lados mientras inspecciona (si tiene personalidad curiosa)
            float curiosity = (_ocean.O + _ocean.N) / 2f;
            if (Random.value < curiosity * Time.deltaTime * 0.5f)
                _rotation.LookAtRandomAngle(transform.rotation, 60f);

            // ¿El jugador está aquí escondido? → INSTA LOSS
           if (IsPlayerHidingHere(_currentTarget))
            {
                Debug.Log("[Vagabundo] ¡Jugador encontrado!");
                
                HidingInteraction hiding = _currentTarget.GetComponent<HidingInteraction>();
                if (hiding != null) hiding.ForceExit();
                
                StartCoroutine(KillPlayerWithDelay());
                StopSearch();  // ← solo en HideoutBehaviour, quítalo en PatrolHomeless
                return;
            }

            if (_inspectTimer <= 0f)
            {
                // No estaba aquí — ir al siguiente
                _searchedCount++;
                _currentTarget = null;
                _inspecting    = false;

                bool reachedLimit = maxHideoutsToSearch > 0 && _searchedCount >= maxHideoutsToSearch;
                if (_searchQueue.Count == 0 || reachedLimit)
                    StopSearch();
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  LÓGICA PRINCIPAL
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Llamado por DetectionEvents cuando el jugador sale del cono de visión.
    /// Decide si vale la pena buscar en escondites según la personalidad OCEAN.
    /// </summary>
    void OnPlayerHidden()
    {
        if (_isSearching) return;

        // Conscientiousness alta → más probabilidad de ir a revisar
        // Openness alta          → más curioso, también revisa
        float searchChance = 0.3f + _ocean.C * 0.5f + _ocean.O * 0.2f;

        if (Random.value > searchChance)
        {
            Debug.Log("[HideoutBehaviour] Perdió al jugador pero decidió no buscar (personalidad).");
            return;
        }

        StartSearch();
    }

    void StartSearch()
    {
        if (hideouts == null || hideouts.Count == 0) return;

        _isSearching   = true;
        _searchedCount = 0;
        _inspecting    = false;

        // Orden aleatorio de escondites a revisar
        _searchQueue = new List<Transform>(hideouts);
        for (int i = 0; i < _searchQueue.Count; i++)
        {
            int randomIndex = Random.Range(i, _searchQueue.Count);
            Transform temp = _searchQueue[i];
            _searchQueue[i] = _searchQueue[randomIndex];
            _searchQueue[randomIndex] = temp;
        }

        PickNextHideout();
        Debug.Log("[HideoutBehaviour] Iniciando búsqueda en escondites.");
    }

    void PickNextHideout()
    {
        if (_searchQueue.Count == 0)
        {
            StopSearch();
            return;
        }

        _currentTarget = _searchQueue[0];
        _searchQueue.RemoveAt(0);

        _aStar.destination = _currentTarget.position;
        _inspecting        = false;

        Debug.Log("[HideoutBehaviour] Yendo a revisar: " + _currentTarget.name);
    }

    void StartInspecting()
    {
        _inspecting   = true;
        _inspectTimer = inspectDuration;
        _aStar.destination = transform.position; // Detener movimiento
        Debug.Log("[HideoutBehaviour] Inspeccionando: " + _currentTarget.name);
    }

    void StopSearch()
    {
        _isSearching   = false;
        _currentTarget = null;
        _inspecting    = false;
        Debug.Log("[HideoutBehaviour] Búsqueda terminada, volviendo a patrulla.");
    }

    // ══════════════════════════════════════════════════════════════════════
    //  DETECCIÓN EN ESCONDITE
    // ══════════════════════════════════════════════════════════════════════

    bool IsPlayerHidingHere(Transform hideout)
    {
        HidingInteraction hiding = hideout.GetComponent<HidingInteraction>();
        if (hiding == null) return false;
        return hiding.IsHiding;
    }

    // ══════════════════════════════════════════════════════════════════════
    //  GIZMOS
    // ══════════════════════════════════════════════════════════════════════
    void OnDrawGizmos()
    {
        if (hideouts == null) return;
        foreach (var h in hideouts)
        {
            if (h == null) continue;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawWireSphere(h.position, arrivalDistance);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
            Gizmos.DrawLine(transform.position, h.position);
        }

        if (_isSearching && _currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_currentTarget.position, 0.3f);
        }
    }

    IEnumerator KillPlayerWithDelay()
    {
        // Esperar un frame para que el Animator se inicialice tras el ForceExit
        yield return null;
        yield return new WaitForSeconds(0.1f);
        
        // Ahora sí drenar la vida
        for (int i = 0; i < 100; i++)
            DetectionEvents.RaisePlayerDetected();
    }
}