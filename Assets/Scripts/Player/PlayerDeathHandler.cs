using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerDetection playerDetection;
    [SerializeField] private GameObject deathScreen;

    [Header("Opciones")]
    [SerializeField] private bool pauseGameOnDeath = true;

    private bool isDead = false;

    void OnEnable()
    {
        if (playerDetection != null)
        {
            playerDetection.healthChanged += CheckDeath;
        }
    }

    void OnDisable()
    {
        if (playerDetection != null)
        {
            playerDetection.healthChanged -= CheckDeath;
        }
    }

    void CheckDeath(float currentHealth)
    {
        if (isDead) return;

        if (currentHealth <= 0)
        {
            isDead = true;
            OnPlayerDeath();
        }
    }

    void OnPlayerDeath()
    {
        Debug.Log("Te vieron.");

        // Activar Death Screen
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No asignaste el Death Screen en el inspector.");
        }

        // Pausar juego (opcional)
        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }

    }
}