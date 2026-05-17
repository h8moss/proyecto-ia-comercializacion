using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomCarSprite : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    void Awake()
    {
        if (sprites.Length == 0) return;
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
