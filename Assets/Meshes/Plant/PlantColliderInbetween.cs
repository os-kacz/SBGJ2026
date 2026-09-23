using UnityEngine;

public class PlantColliderInbetween : MonoBehaviour
{

    public GameObject parentObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // check if has a scriptable object of type SOItem


        // check if the item AND the parentObject is null or not

        // call PlantLogic.OnItemConsumed() with the item scriptable object and the ItemScript
    }
}
