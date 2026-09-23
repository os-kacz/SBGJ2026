using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    [Header("Item Data")]
    public ItemData itemData;

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("No Item Data Assigned");
        }

    }
}
