using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerController playerObject;

    // Current health
    [SerializeField] GameObject healthBarObject;
    RectTransform healthBarTransform;
    Image healthBarImage;

    // Limited health
    [SerializeField] GameObject healthLimitBarObject;
    RectTransform healthLimitBarTransform;
    Image healthLimitBarImage;

    public float maxWidth = 2;
    private float healthLimit;
    private float health;
    private float maxHealth;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        healthBarTransform = healthBarObject.GetComponent<RectTransform>();
        healthBarImage = healthBarObject.GetComponent<Image>();

        healthLimitBarTransform = healthLimitBarObject.GetComponent<RectTransform>();
        healthLimitBarImage = healthLimitBarObject.GetComponent<Image>();

        health = playerObject.health;
        healthLimit = playerObject.healthLimit;

        maxHealth = playerObject.maxHealth;
    }

    void Update()
    {
        health = playerObject.health;
        healthLimit = playerObject.healthLimit;

        float healthPercent = Mathf.Clamp01(health / maxHealth);
        float maxHealthPercent = Mathf.Clamp01(healthLimit / maxHealth);

        healthBarTransform.localScale = new Vector3(maxWidth * healthPercent, healthBarTransform.localScale.y, healthBarTransform.localScale.z);
        // print("Current health percentage: " + healthPercent);
        healthLimitBarTransform.localScale = new Vector3(maxWidth * maxHealthPercent, healthLimitBarTransform.localScale.y, healthLimitBarTransform.localScale.z);
        // print("Current health limit percentage: " + maxHealthPercent);

        if (health < 8)
        {
            healthBarImage.color = new Color(0.7373f, 0.0588f, 0f);
        }
        else if (health < 16)
        {
            healthBarImage.color = new Color(0.949f, 0.627f, 0.0196f);
        }
        else
        {
            healthBarImage.color = new Color(0.0118f, 0.5843f, 0f);
        }

        if (healthLimit < 8)
        {
            healthLimitBarImage.color = new Color(0.7373f, 0.0588f, 0f);
        }
        else if (healthLimit < 16)
        {
            healthLimitBarImage.color = new Color(0.949f, 0.627f, 0.0196f);
        }
        else
        {
            healthLimitBarImage.color = new Color(0.0118f, 0.5843f, 0f);
        }
    }
}