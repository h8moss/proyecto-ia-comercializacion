using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoomHealthIndicator : MonoBehaviour
{
    [SerializeField] private List<Sprite> healthImages;

    private Image img;
    private float maxHealth;
    private PlayerDetection pd;

    void Start()
    {
        pd = PlayerLocator.Player.GetComponent<PlayerDetection>();
        pd.healthChanged += healthChanged;
        maxHealth = pd.MaxHealth;
        img = GetComponent<Image>();
    }

    void OnDestroy()
    {
        pd.healthChanged -= healthChanged;
    }

    void healthChanged(float v)
    {
        if (healthImages.Count == 0 || maxHealth <= 0)
            return;

        float normalized = Mathf.Clamp01(v / maxHealth);

        int index = Mathf.FloorToInt(normalized * (healthImages.Count - 1));

        img.sprite = healthImages[index]; 
    }
}
