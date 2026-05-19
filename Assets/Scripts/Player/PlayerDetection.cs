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
    private bool isDead = false;
    private float healTimer = 0f;
    private bool isHiddenOnItem = false;

    private HidingBehaviour hb;

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
        hb = GetComponent<HidingBehaviour>();

        hb.OnHidden += Hidden;
        hb.OnUnhidden += Unhidden;
        DetectionEvents.OnPlayerDetected += OnDetected;
        DetectionEvents.OnPlayerHidden += OnNotSeen;

        health = maxHealth;
    }

    void OnDestroy() 
    {
        hb.OnHidden -= Hidden;
        hb.OnUnhidden -= Unhidden;
        DetectionEvents.OnPlayerDetected -= OnDetected;
        DetectionEvents.OnPlayerHidden -= OnNotSeen;
    }

    void Update()
    {
        if (godMode) return;

        if (!isHiddenOnItem)
            Health -= detectionCount * detectionRate * Time.deltaTime;

        if (healTimer <= 0f)
        {
            float newHealRate = healRate * (isHiddenOnItem ? 1.5f : 1f);
            Health = Mathf.Min(maxHealth, health + newHealRate*Time.deltaTime);
        } else
        {
            healTimer -= Time.deltaTime * (isHiddenOnItem ? 2f : 1f);
        }
    }

    void OnDetected()
    {
        detectionCount++;
        healTimer = healDelay;
    }

    void OnNotSeen()
    {
         detectionCount--;
         if (gameObject.activeInHierarchy)   // ← solo agregar este if
            healTimer = healDelay;
    }

    void Hidden()
    {
        isHiddenOnItem = true;
    }
    
    void Unhidden()
    {
        isHiddenOnItem = false;
    }
}
