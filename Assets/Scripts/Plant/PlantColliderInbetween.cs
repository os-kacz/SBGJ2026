using UnityEngine;

public class PlantColliderInbetween : MonoBehaviour
{

    public PlantLogic parentObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // check if has a scriptable object of type SOItem
        ItemPickup itemPickup = other.gameObject.GetComponent<ItemPickup>();

        Debug.Log(itemPickup);
        if (itemPickup != null && parentObject != null)
        {
            // call PlantLogic.OnItemConsumed() with the item scriptable object and the ItemScript
            parentObject.OnItemConsumed(itemPickup, itemPickup.gameObject);
            Debug.Log("ConsumedAsk");

        }
    }
}
