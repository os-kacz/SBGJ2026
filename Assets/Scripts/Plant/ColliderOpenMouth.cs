using UnityEngine;
public class ColliderOpenMouth : MonoBehaviour
{

    public PlantLogic plantLogic;
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
        if (other.CompareTag("Item"))
        {
            plantLogic.OpenMouth();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            plantLogic.CloseMouth();
        }
    }
}
