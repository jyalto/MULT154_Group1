using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerController playerObject;
    private RectTransform healthBarTransform;
    private Image healthBarImage;
    public float maxWidth = 1.70885551f;
    public float currentHealth;
    public float maxHealth = 25;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        healthBarTransform = GetComponent<RectTransform>();
        healthBarImage = GetComponent<Image>();
    }

    void Update()
    {
        currentHealth = playerObject.health;
        float percent = Mathf.Clamp01(currentHealth / maxHealth);

        float healthPercentage = currentHealth / maxHealth;
        healthBarTransform.localScale = new Vector3(maxWidth * percent, healthBarTransform.localScale.y, healthBarTransform.localScale.z);

        if (currentHealth < 8)
        {
            healthBarImage.color = new Color(0.7373f, 0.0588f, 0f);
        }
        else if (currentHealth < 16)
        {
            healthBarImage.color = new Color(0.949f, 0.627f, 0.0196f);
        }
        else
        {
            healthBarImage.color = new Color(0.0118f, 0.5843f, 0f);
        }
    }
}