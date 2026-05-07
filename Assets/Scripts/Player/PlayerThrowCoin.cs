using System;
using System.Collections;
using UnityEngine;

public class PlayerThrowCoin : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float fireReloadTime;
    [SerializeField] private float fireForce;
    [SerializeField] private int maxCoinCount;
    
    private int coinCount;
    private bool canFire = true;

    public int MaxCoinCount
    {
        get => maxCoinCount;
    }

    public int CoinCount
    {
        get => coinCount;
    }

    public Action<int> coinsChanged;

    void Start()
    {
        canFire = true;
        coinCount = maxCoinCount;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canFire && coinCount > 0)
        {
            StartCoroutine(Fire());
        }
    }

    private Vector2 GetCursorDirection()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mouseWorld - (Vector2)transform.position).normalized;
    }

    IEnumerator Fire()
    {
        canFire = false;

        coinCount--;
        coinsChanged.Invoke(coinCount);

        var coin = Instantiate(coinPrefab); // TODO: Object pooler
        coin.GetComponent<CoinController>().ResetCoin(
            (Vector2)transform.position + GetCursorDirection(),
            (Vector2)transform.position + GetCursorDirection() * fireForce
        );
        yield return new WaitForSeconds(fireReloadTime);
        canFire = true;
    }
}
