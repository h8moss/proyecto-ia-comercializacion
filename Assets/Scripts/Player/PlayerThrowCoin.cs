using System.Collections;
using UnityEngine;

public class PlayerThrowCoin : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float fireReloadTime;
    [SerializeField] private float fireForce;
    
    private bool canFire = true;
    void Start()
    {
        canFire = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canFire)
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
        var coin = Instantiate(coinPrefab); // TODO: Object pooler
        coin.GetComponent<CoinController>().ResetCoin(
            (Vector2)transform.position + GetCursorDirection(),
            (Vector2)transform.position + GetCursorDirection() * fireForce
        );
        yield return new WaitForSeconds(fireReloadTime);
        canFire = true;
    }
}
