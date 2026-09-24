using System.Collections;
using UnityEngine;

public class ExplosionHazard : MonoBehaviour
{
    [Header("Settings")] 
    public int damage = 20;
    public float explosionRadius = 5f; // Radius of the explosion

    private bool hasExploded = false;

    private void Start()
    {
        Explode();
    }

    private void Explode()
    {
        if(hasExploded) return; // Prevent multiple explosions)
        hasExploded = true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius); // Adjust the radius as needed
        
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Debug.Log("Knockback");
                    playerController.ApplyKnockback(transform.position);
                }
            }
        }

        Destroy(gameObject, 2f); // Destroy the explosion hazard after it has exploded, giving time for the VFX to go off
    }

}
