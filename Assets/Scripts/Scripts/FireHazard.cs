using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 5;
    public float damageInterval = 0.25f; // Time in seconds between damage applications
    public float hazardLifetime = 10f; // can be adjusted to link with plant stats too

    private float nextDamageTime = 0f;
    private void Start()
    {
        Destroy(gameObject, hazardLifetime); // Destroy the hazard after its lifetime expires
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextDamageTime = 0f; // Reset the damage timer when the player exits the hazard
        }
    }
}
