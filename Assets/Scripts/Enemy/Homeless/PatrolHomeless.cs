using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

/// <summary>
/// PatrolHomeless.cs — Patrullaje del Vagabundo entre escondites.
///
/// COMPORTAMIENTO:
///   - Va a escondites en orden aleatorio
///   - Al llegar: a veces inspecciona, a veces pasa de largo (depende si lo revisó recientemente)
///   - Si inspecciona y el jugador está dentro → insta loss (lógica en VagabundoHelper)
///   - Reacciona a sonidos (igual que PatrolBehaviour original)
/// </summary>
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
public class PatrolHomeless : MonoBehaviour
{
    // [Header("Escondites a patrullar")]
    // [Tooltip("Lista de escondites entre los que patrulla.")]
    // [SerializeField] private List<Transform> hideouts;

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

    private List<Transform> Hideouts { get => HidingObjectRegister.Instance?.HidingObjects; }

    // ─────────────────────────────────────────────────────────────────────


    void Start()
    {
        _aStar    = GetComponent<AIPath>();
        _rotation = GetComponent<GeneralEnemyRotation>();
        _ocean    = GetComponent<OceanManager>();

        WorldEvents.OnSoundMade += HandleSoundMade;

        PickRandomHideout();
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
    }

    void Update()
    {
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

        _rotation.LookAt(_currentTarget.position);

        float dist = Vector2.Distance(transform.position, _currentTarget.position);
        bool arrived = dist <= arrivalDistance || _aStar.reachedDestination;

        if (arrived)
        {
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

        float curiosity = (_ocean.O + _ocean.N) / 2f;
        if (curiosity > 0.3f && Random.value < curiosity * Time.deltaTime * 0.5f)
            _rotation.LookAtRandomAngle(transform.rotation, 60f);

        // ¿El jugador está aquí? → INSTA LOSS
        if (VagabundoHelper.IsPlayerHidingHere(_currentTarget))
        {
            Debug.Log("[VagabundoPatrol] ¡Jugador encontrado durante patrulla! INSTA LOSS");
            StartCoroutine(VagabundoHelper.KillPlayer(_currentTarget));
            return;
        }

        if (_inspectTimer <= 0f)
        {
            MarkAsVisited(_currentTargetIndex);
            PickRandomHideout();
        }
    }

    void UpdateInvestigating()
    {
        _rotation.LookAt(_investigationPoint);

        // Si pasa cerca de un escondite con jugador dentro → te encuentra
        foreach (var h in Hideouts)
        {
            if (h == null) continue;
            float distToHide = Vector2.Distance(transform.position, h.position);
            if (distToHide <= arrivalDistance && VagabundoHelper.IsPlayerHidingHere(h))
            {
                Debug.Log("[VagabundoPatrol] ¡Te encontró durante investigación de sonido!");
                StartCoroutine(VagabundoHelper.KillPlayer(h));
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
        if (Hideouts == null || Hideouts.Count == 0)
        {
            Debug.LogWarning("[VagabundoPatrol] No hay escondites asignados.");
            return;
        }

        int newIndex;
        if (Hideouts.Count == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, Hideouts.Count);
            } while (newIndex == _currentTargetIndex);
        }

        _currentTargetIndex = newIndex;
        _currentTarget      = Hideouts[newIndex];
        _aStar.destination  = _currentTarget.position;
        _state              = VagabundoState.Patrolling;
    }

    void StartInspecting()
    {
        _state         = VagabundoState.Inspecting;
        _inspectTimer  = inspectDuration;
        _aStar.destination = transform.position;
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
    //  SONIDO (mismo patrón que PatrolBehaviour original)
    // ══════════════════════════════════════════════════════════════════════

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);

        float n = _ocean.Neuroticism;
        float threshold = Mathf.Lerp(8f, 0.3f, n * n * n); // Much less aggressive to sounds
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
        if (Hideouts == null) return;

        Gizmos.color = new Color(0.4f, 0.7f, 1f, 0.4f);
        foreach (var h in Hideouts)
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
}