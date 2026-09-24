using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Auto-Heal Settings")]

    public int healAmount = 20; // Amount to heal each interval
    public float healInterval = 2f; // Time in seconds between heals
    public float healDelay = 1f; // Delay before healing starts after taking damage

    private float lastDamageTime = -9999f; // Time when the player last took damage
    private float nextHealTime = 0f; // Time when the next heal should occur

    [Header("Vignetter Settings")]
    public Image vignetteImage;
    public float vignetteMaxIntensity = 0.8f; // Maximum intensity of the vignette effect
    public float LerpSpeed = 5f; // Speed at which the vignette effect changes

    [Header("CameraShake")]
    public CameraShake cameraShake;

    private float currentVignetteAlpha = 0f; // Current alpha value of the vignette effect
    private float targetVignetteAlpha = 0f; // Target alpha value of the vignette effect

    private GameManager gameManager;

    void Start()
    {
        currentHealth = maxHealth;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        lastDamageTime = Time.time;
        
        if (cameraShake != null)
        {
            cameraShake.TriggerShake();
        }
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        gameManager.GameOver();
    }

    void Update()
    {
        HealPlayer();
        UpdateVignette();
    }

    public void UpdateVignette()
    {
        if (vignetteImage == null) return;

        float healthPercentage = 1f -((float)currentHealth / maxHealth);
        targetVignetteAlpha = vignetteMaxIntensity * healthPercentage;

        currentVignetteAlpha = Mathf.Lerp(currentVignetteAlpha, targetVignetteAlpha, Time.deltaTime * LerpSpeed);

        Color colour = vignetteImage.color;
        colour.a = currentVignetteAlpha;
        vignetteImage.color = colour;

    }

    private void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            if (Time.time >= lastDamageTime + healDelay && Time.time >= nextHealTime)
            {
                currentHealth += healAmount;
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;
                nextHealTime = Time.time + healInterval; // Schedule the next heal
            }
        }

       

    }
}
