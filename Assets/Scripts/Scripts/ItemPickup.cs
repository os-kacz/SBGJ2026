using Unity.Mathematics;
using UnityEngine;
public class ItemPickup : MonoBehaviour
{

    [Header("Item Data")]
    public ItemData itemData;
    public bool beingConsumed = false;

    private float minimumDecay = 0.1f;
    private float currentDecay = 1.0f;
    
    private float currentDecayRate = 0.25f; 

    private Vector3 originalScale = Vector3.zero;
    private Vector3 originalPosition = Vector3.zero;

    private Vector3 newDestination = Vector3.zero;

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("No Item Data Assigned");
        }

        originalScale = gameObject.transform.localScale;

    }

    void Update()
    {
        if (beingConsumed && (currentDecay > minimumDecay))
        {
            currentDecay -= currentDecayRate * Time.deltaTime;
            gameObject.transform.localScale = currentDecay * originalScale;
            gameObject.transform.position = Vector3.Lerp(newDestination, originalPosition, currentDecay);
        }
        else if (currentDecay <= minimumDecay)
        {
            Destroy(gameObject);
        }
    }

    public void OnConsumed(float decayRate, Vector3 destination)
    {
        currentDecayRate = decayRate;
        beingConsumed = true;
        newDestination = destination;
        originalPosition = gameObject.transform.position;
        gameObject.tag = "Untagged";
    }
}
