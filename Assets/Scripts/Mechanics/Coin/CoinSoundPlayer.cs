using UnityEngine;

[RequireComponent(typeof(CoinController))]
public class CoinSoundPlayer : MonoBehaviour
{
    [SerializeField] private float loudness;

    private CoinController cc;
    void Start()
    {
        cc = GetComponent<CoinController>();
        cc.OnCollision += MakeSoundCol;
        cc.OnLanded +=    MakeSound;
    }

    void OnDestroy()
    {
        cc.OnCollision -= MakeSoundCol;
        cc.OnLanded -= MakeSound;
    }

    void MakeSound()
    {
        WorldEvents.RaiseSoundMade(transform.position, loudness);
    }

    void MakeSoundCol(Collider2D c)
    {
        MakeSound();
    }
}
