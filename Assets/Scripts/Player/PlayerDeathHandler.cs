using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject deathScreen;

    [Header("Opciones")]
    [SerializeField] private bool pauseGameOnDeath = true;
    [SerializeField] private float deathTimer = 3.0f;

    private bool isDead = false;

    private PlayerDetection playerDetection;

    void Start()
    {
        playerDetection = PlayerLocator.Player.GetComponent<PlayerDetection>();
        playerDetection.healthChanged += CheckDeath;
    }

    void OnDestroy()
    {
        if (playerDetection != null)
        {
            playerDetection.healthChanged -= CheckDeath;
        }
        Time.timeScale = 1;
    }

    void CheckDeath(float currentHealth)
    {
        if (isDead) return;

        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(OnPlayerDeath());
        }
    }

    IEnumerator OnPlayerDeath()
    {
        yield return new WaitForSeconds(deathTimer);

        // Activar Death Screen
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No asignaste el Death Screen en el inspector.");
        }

        // Pausar juego (opcional) // Por que es opcional?? Cuando no querriamos pausar?
        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }

    }
}