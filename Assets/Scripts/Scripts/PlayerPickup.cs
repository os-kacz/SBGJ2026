using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip throwSound;

    [Header("References")]
    public Transform holdingPosition;

    [Header("Settings")]
    public float pickupRange = 2f;
    public float throwForce = 15f;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private bool isHoldingItem = false;

    void Update()
    {
        if(InputSystem.actions["Pickup"].WasPressedThisFrame())
        {
            if (heldItem == null) TryPickup();
            else DropItem();
        }

        if (InputSystem.actions["Throw"].WasPressedThisFrame() && heldItem != null)
        {
            ThrowItem();
        }
    }
    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
           if(hit.collider.CompareTag("Item"))
           {
                isHoldingItem = true;
                PlayPickUpSound();
               heldItem = hit.collider.gameObject;
               heldItemRb = heldItem.GetComponent<Rigidbody>();

               heldItemRb.isKinematic = true;
               heldItemRb.useGravity = false;

               heldItem.transform.position = holdingPosition.position;
               heldItem.transform.rotation = holdingPosition.rotation;
               heldItem.transform.SetParent(holdingPosition);
           }
           else
           {
               Debug.Log("No item to pick up");
           }

        }
    }

    void ThrowItem()
    {
        PlayThrowSound();
        heldItem.transform.SetParent(null);
        heldItemRb.isKinematic = false;
        heldItemRb.useGravity = true;

        Vector3 throwDirection = transform.forward * throwForce + transform.up / 2f;

        heldItemRb.AddForce(throwDirection, ForceMode.Impulse);

        heldItem = null;
        heldItemRb = null;

    }

    public void DropItem()
    {
        isHoldingItem = false;
        heldItem.transform.SetParent(null);
        heldItemRb.isKinematic = false;
        heldItemRb.useGravity = true;
        heldItem = null;
        heldItemRb = null;
    }

    public GameObject GetHeldItemData()
    {
        return heldItem;
    }

    public void PlayPickUpSound()
    {
        if (audioSource != null && pickupSound != null)
        {
            if (isHoldingItem)
            {
                audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
                audioSource.PlayOneShot(pickupSound);
            }
        }
    }

    public void PlayThrowSound()
    {
        if (audioSource != null && throwSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(throwSound);
        }
    }

}
