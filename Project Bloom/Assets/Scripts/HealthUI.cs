using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerController playerObject;

    [SerializeField] GameObject healthBarObject;
    RectTransform healthBarTransform;
    Image healthBarImage;

    [SerializeField] GameObject maxHealthBarObject;
    RectTransform maxHealthBarTransform;
    Image maxHealthBarImage;

    public float maxWidth = 1.70885551f;
    private float maxHealth;
    private float currentHealth;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        healthBarTransform = healthBarObject.GetComponent<RectTransform>();
        healthBarImage = healthBarObject.GetComponent<Image>();

        maxHealthBarTransform = maxHealthBarObject.GetComponent<RectTransform>();
        maxHealthBarImage = maxHealthBarObject.GetComponent<Image>();

        maxHealth = playerObject.maxHealth;
        currentHealth = playerObject.health;
    }

    void Update()
    {
        currentHealth = playerObject.health;
        currentHealth = playerObject.maxHealth;

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