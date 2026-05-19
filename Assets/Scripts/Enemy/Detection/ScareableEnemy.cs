using System;
using System.Collections;
using UnityEngine;

public class ScareableEnemy : MonoBehaviour
{
    [SerializeField] private float scareDuration = 3.0f;
    private bool isScared = false;

    public bool IsScared
    {
        get => isScared;
    }

    public float Multiplier { get => isScared ? 1.0f : 0.0f; }
    public float Inverse { get => 1.0f - Multiplier; }

    public Action<bool> ScaredChanged;

    public void Scare() {
        StartCoroutine(Scared());
    }

    IEnumerator Scared()
    {
        isScared = true;
        ScaredChanged?.Invoke(true);
        yield return new WaitForSeconds(scareDuration);
        isScared = false;
        ScaredChanged?.Invoke(false);
    }
}
