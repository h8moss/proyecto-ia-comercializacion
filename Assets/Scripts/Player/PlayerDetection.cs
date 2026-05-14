using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float detectionRate;
    [SerializeField] private float healDelay;
    [SerializeField] private float healRate;
    [SerializeField] private bool godMode;

    private int detectionCount = 0;
    private float health;
    private bool canHeal = true;
    private bool isDead = false;

    public Action<float> healthChanged;
    public Action OnDeath;

    public float Health
    {
        get => health;
        private set
        {   
            if (value != health)
            {
                health = value;
                healthChanged?.Invoke(value);

                if (health <= 0 && !isDead)
                {
                    isDead = true;
                    OnDeath.Invoke();
                }
            }
        }
    }
    public float MaxHealth
    {
        get => maxHealth;
    }

    void Start()
    {
        DetectionEvents.OnPlayerDetected += OnDetected;
        DetectionEvents.OnPlayerHidden += OnHidden;

        health = maxHealth;
    }

    void OnDestroy() 
    {
        DetectionEvents.OnPlayerDetected -= OnDetected;
        DetectionEvents.OnPlayerHidden -= OnHidden;
    }

    void Update()
    {
        if (godMode) return;
        Health -= detectionCount * detectionRate * Time.deltaTime;
        if (canHeal)
        {
            Health = Mathf.Min(maxHealth, health + healRate*Time.deltaTime);
        }
    }

    void OnDetected()
    {
        detectionCount++;
        canHeal = false;
        StopCoroutine(ResumeHealing());
    }

    void OnHidden()
    {
        detectionCount--;
        StartCoroutine(ResumeHealing());
    }

    IEnumerator ResumeHealing()
    {
        yield return new WaitForSeconds(healDelay);
        canHeal = true;
    }
}
