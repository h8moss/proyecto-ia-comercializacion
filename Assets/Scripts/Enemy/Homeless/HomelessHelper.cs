using System.Collections;
using UnityEngine;

/// <summary>
/// VagabundoHelper.cs — Métodos compartidos entre los scripts del Vagabundo.
///
/// Centraliza la lógica que usaban PatrolHomeless y HideoutBehaviour para evitar
/// duplicar código. Si en el futuro cambia la forma de matar al jugador o detectar
/// si está escondido, solo se modifica aquí.
/// </summary>
public static class VagabundoHelper
{
    /// <summary>
    /// Verifica si el jugador está escondido en el escondite dado.
    /// </summary>
    public static bool IsPlayerHidingHere(Transform hideout)
    {
        if (hideout == null) return false;
        HidingInteraction hiding = hideout.GetComponent<HidingInteraction>();
        if (hiding == null) return false;
        return hiding.IsHiding;
    }

    /// <summary>
    /// Saca al jugador del escondite y lo mata después de un pequeño delay
    /// para permitir que se dispare la animación de muerte.
    /// </summary>
    public static IEnumerator KillPlayer(Transform hideout)
    {
        // 1. Sacar al jugador del escondite
        if (hideout != null)
        {
            HidingInteraction hiding = hideout.GetComponent<HidingInteraction>();
            if (hiding != null) hiding.ForceExit();
        }

        // 2. Esperar un frame y un pequeño delay para que el Animator se inicialice
        yield return null;
        yield return new WaitForSeconds(0.1f);

        // 3. Drenar la vida del jugador
        for (int i = 0; i < 100; i++)
            DetectionEvents.RaisePlayerDetected();
    }
}