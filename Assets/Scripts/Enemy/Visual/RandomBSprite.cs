using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomBSprite : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float ASpriteProbability = 0.5f;

    void Start()
    {
        Animator animator = GetComponent<Animator>();
        bool shouldBeB = Random.value > ASpriteProbability;
        Debug.Log(shouldBeB);
        animator.SetBool("IsB", shouldBeB);
        animator.SetTrigger("Reset");
    }
}
