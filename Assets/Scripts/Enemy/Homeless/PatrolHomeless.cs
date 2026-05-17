using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

/// <summary>
/// PatrolHomeless.cs — Patrullaje específico del Vagabundo entre escondites.
///
/// COMPORTAMIENTO:
///   - Va a escondites en orden aleatorio
///   - Al llegar: a veces inspecciona, a veces pasa de largo (depende si lo revisó recientemente)
///   - Si inspecciona y el jugador está dentro → insta loss
///   - Reacciona a sonidos (igual que PatrolBehaviour original)
///   - Si ve al jugador correr y lo pierde, prioriza el escondite más cercano a esa posición
///     (esto lo maneja HideoutBehaviour, no este script)
///
/// NO USA PatrolBehaviour. Reemplázalo en el GameObject Vagabundo.
/// </summary>
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
public class PatrolHomeless : MonoBehaviour
{
    [Header("Escondites a patrullar")]
    [Tooltip("Lista de escondites entre los que patrulla. Pueden ser los mismos que en HideoutBehaviour.")]
    [SerializeField] private List<Transform> hideouts;

    [Header("Inspección")]
    [Tooltip("Probabilidad base de detenerse a inspeccionar al llegar a un escondite (0-1).")]
    [Range(0f, 1f)]
    [SerializeField] private float inspectChance = 0.6f;

    [Tooltip("Segundos que recuerda haber visitado un escondite. Mientras esté en memoria, no se detiene.")]
    [SerializeField] private float memoryDuration = 15f;

    [Tooltip("Tiempo que se queda inspeccionando cada escondite.")]
    [SerializeField] private float inspectDuration = 2.5f;

    [Tooltip("Radio para considerar que llegó al escondite.")]
    [SerializeField] private float arrivalDistance = 0.8f;

    [Header("Sonido")]
    [Tooltip("Distancia máxima a la que escucha sonidos.")]
    [SerializeField] private float maxHearingDistance = 10f;

    // ── Estados internos ──────────────────────────────────────────────────
    private enum VagabundoState { Patrolling, Inspecting, Investigating }
    private VagabundoState _state = VagabundoState.Patrolling;

    // ── Componentes ───────────────────────────────────────────────────────
    private AIPath               _aStar;
    private GeneralEnemyRotation _rotation;
    private OceanManager         _ocean;

    // ── Memoria de escondites visitados ───────────────────────────────────
    private Dictionary<int, float> _recentlyVisited = new();

    // ── Variables de estado ───────────────────────────────────────────────
    private Transform _currentTarget;
    private int       _currentTargetIndex;
    private float     _inspectTimer;
    private Vector3   _investigationPoint;

    // ─────────────────────────────────────────────────────────────────────

    void Start()
    {
        _aStar    = GetComponent<AIPath>();
        _rotation = GetComponent<GeneralEnemyRotation>();
        _ocean    = GetComponent<OceanManager>();

        // Escuchar sonidos del mundo
        WorldEvents.OnSoundMade += HandleSoundMade;

        // Empezar yendo a un escondite aleatorio
        PickRandomHideout();
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
    }

    void Update()
    {
        // Actualizar memoria (los escondites se "olvidan" con el tiempo)
        UpdateMemory();

        switch (_state)
        {
            case VagabundoState.Patrolling:    UpdatePatrolling();    break;
            case VagabundoState.Inspecting:    UpdateInspecting();    break;
            case VagabundoState.Investigating: UpdateInvestigating(); break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  ESTADOS
    // ══════════════════════════════════════════════════════════════════════

    void UpdatePatrolling()
    {
        if (_currentTarget == null)
        {
            PickRandomHideout();
            return;
        }

        // Mirar hacia el destino
        _rotation.LookAt(_currentTarget.position);

        float dist = Vector2.Distance(transform.position, _currentTarget.position);

        // Llegó si: está dentro del radio O el AIPath dice que llegó
        bool arrived = dist <= arrivalDistance || _aStar.reachedDestination;

        if (arrived)
        {
            // ¿Detenerse a inspeccionar o pasar de largo?
            bool wasVisitedRecently = _recentlyVisited.ContainsKey(_currentTargetIndex);
            float adjustedChance = inspectChance + _ocean.C * 0.2f;

            if (!wasVisitedRecently && Random.value < adjustedChance)
            {
                StartInspecting();
            }
            else
            {
                MarkAsVisited(_currentTargetIndex);
                PickRandomHideout();
            }
        }
    }

    void UpdateInspecting()
    {
        _inspectTimer -= Time.deltaTime;

        // Mirar hacia los lados mientras inspecciona (curiosidad según personalidad)
        float curiosity = (_ocean.O + _ocean.N) / 2f;
        if (curiosity > 0.3f && Random.value < curiosity * Time.deltaTime * 0.5f)
            _rotation.LookAtRandomAngle(transform.rotation, 60f);

        // ¿El jugador está escondido aquí? → INSTA LOSS
        if (IsPlayerHidingHere(_currentTarget))
        {
            Debug.Log("[Vagabundo] ¡Jugador encontrado!");
            
            HidingInteraction hiding = _currentTarget.GetComponent<HidingInteraction>();
            if (hiding != null) hiding.ForceExit();
            
            StartCoroutine(KillPlayerWithDelay());
            return;
        }

        if (_inspectTimer <= 0f)
        {
            // Termina inspección, marca como visitado y sigue
            MarkAsVisited(_currentTargetIndex);
            PickRandomHideout();
        }
    }

    void UpdateInvestigating()
    {
        _rotation.LookAt(_investigationPoint);

        // Revisar si pasa cerca de algún escondite con jugador escondido
        foreach (var h in hideouts)
        {
            if (h == null) continue;
            float distToHide = Vector2.Distance(transform.position, h.position);
            if (distToHide <= arrivalDistance && IsPlayerHidingHere(h))
            {
                Debug.Log("[VagabundoPatrol] ¡Te encontró durante investigación de sonido!");

                HidingInteraction hiding = h.GetComponent<HidingInteraction>();
                if (hiding != null) hiding.ForceExit();

                for (int i = 0; i < 100; i++)
                    DetectionEvents.RaisePlayerDetected();
                return;
            }
        }

        float dist = Vector2.Distance(transform.position, _investigationPoint);
        if (dist <= arrivalDistance)
        {
            Debug.Log("[VagabundoPatrol] Investigación de sonido terminada.");
            _state = VagabundoState.Patrolling;
            PickRandomHideout();
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  SELECCIÓN DE ESCONDITES
    // ══════════════════════════════════════════════════════════════════════

    void PickRandomHideout()
    {
        if (hideouts == null || hideouts.Count == 0)
        {
            Debug.LogWarning("[VagabundoPatrol] No hay escondites asignados.");
            return;
        }

        // Elige uno aleatorio diferente al actual (si hay más de uno)
        int newIndex;
        if (hideouts.Count == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, hideouts.Count);
            } while (newIndex == _currentTargetIndex);
        }

        _currentTargetIndex = newIndex;
        _currentTarget      = hideouts[newIndex];
        _aStar.destination  = _currentTarget.position;
        _state              = VagabundoState.Patrolling;
    }

    void StartInspecting()
    {
        _state         = VagabundoState.Inspecting;
        _inspectTimer  = inspectDuration;
        _aStar.destination = transform.position; // Detenerse
        Debug.Log("[VagabundoPatrol] Inspeccionando: " + _currentTarget.name);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  MEMORIA DE ESCONDITES VISITADOS
    // ══════════════════════════════════════════════════════════════════════

    void MarkAsVisited(int index)
    {
        _recentlyVisited[index] = memoryDuration;
    }

    void UpdateMemory()
    {
        List<int> toRemove = new();
        List<int> keys     = new(_recentlyVisited.Keys);

        foreach (int key in keys)
        {
            _recentlyVisited[key] -= Time.deltaTime;
            if (_recentlyVisited[key] <= 0f)
                toRemove.Add(key);
        }

        foreach (int key in toRemove)
            _recentlyVisited.Remove(key);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  DETECCIÓN
    // ══════════════════════════════════════════════════════════════════════

    bool IsPlayerHidingHere(Transform hideout)
    {
        HidingInteraction hiding = hideout.GetComponent<HidingInteraction>();
        if (hiding == null) return false;
        return hiding.IsHiding;
    }

    // ══════════════════════════════════════════════════════════════════════
    //  SONIDO (mismo patrón que PatrolBehaviour original)
    // ══════════════════════════════════════════════════════════════════════

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);
        float threshold = 0.5f - (_ocean.Neuroticism * 0.4f);
        if (perceivedLoudness < threshold) return;
        if (Random.value >= _ocean.Openness) return;

        Debug.Log("[VagabundoPatrol] ¡Escuchó un sonido en " + position + "!");
        _investigationPoint = position;
        _aStar.destination  = position;
        _state              = VagabundoState.Investigating;
    }

    // ══════════════════════════════════════════════════════════════════════
    //  GIZMOS
    // ══════════════════════════════════════════════════════════════════════

    void OnDrawGizmos()
    {
        if (hideouts == null) return;

        Gizmos.color = new Color(0.4f, 0.7f, 1f, 0.4f);
        foreach (var h in hideouts)
        {
            if (h == null) continue;
            Gizmos.DrawWireSphere(h.position, arrivalDistance);
        }

        if (Application.isPlaying && _currentTarget != null)
        {
            Gizmos.color = _state == VagabundoState.Inspecting ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, _currentTarget.position);
            Gizmos.DrawWireSphere(_currentTarget.position, 0.4f);
        }

        Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, maxHearingDistance);
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