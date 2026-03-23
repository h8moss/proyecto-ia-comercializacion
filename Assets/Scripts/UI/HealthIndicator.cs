using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthIndicator : MonoBehaviour
{
    private Image img;
    private float maxHealth;

    void Start()
    {
        PlayerDetection pd = PlayerLocator.Player.GetComponent<PlayerDetection>();
        pd.healthChanged += healthChanged;
        maxHealth = pd.MaxHealth;
        img = GetComponent<Image>();
    }

    void healthChanged(float v)
    {
        img.fillAmount = v / maxHealth;
    }
}
