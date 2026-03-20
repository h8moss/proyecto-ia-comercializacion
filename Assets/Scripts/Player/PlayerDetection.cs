using System.Collections;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float detectionRate;
    [SerializeField] private float healDelay;
    [SerializeField] private float healRate;

    private int detectionCount = 0;
    [SerializeField] private float health;
    private bool canHeal = true;

    void Start()
    {
        DetectionEvents.OnPlayerDetected += OnDetected;
        DetectionEvents.OnPlayerHidden += OnHidden;

        health = maxHealth;
    }

    void Update()
    {
        health -= detectionCount * detectionRate * Time.deltaTime;
        if (canHeal)
        {
            health = Mathf.Min(maxHealth, health + healRate*Time.deltaTime);
        }

        if (health <= 0)
        {
            // Trigger game over
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
