using UnityEngine;

public class ElectricHazard : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 5;
    public float damageInterval = 1.5f; // Time in seconds between damage applications
    public float hazardLifetime = 10f; // can be adjusted to link with plant stats too

    private float nextDamageTime = 0f;

    void Start()
    {
        Destroy(gameObject, hazardLifetime); // Destroy the hazard after its lifetime expires
    }
    
    void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.SetSlow(true);
        }
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


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.SetSlow(false);
            nextDamageTime = 0f; // Reset the damage timer when the player exits the hazard
        }
    }
}
