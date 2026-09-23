using UnityEngine;
using UnityEngine.AI;

public class PlantLogic : MonoBehaviour
{

    // animation and AI
    private NavMeshAgent agent;
    private Animator animator;

    // hunger variables
    private float hunger  = 0f;
    [SerializeField]
    private float startingHunger = 10f;
    [SerializeField]
    private float maxHunger = 100f;
    [SerializeField]
    private float hungerDecayRate = 0.5f; // Hunger decay rate per second
    

    // item management variables
    [SerializeField]
    private float timeBetweenEvents = 10f; // Time between events in seconds
    private float timeSinceLastEvent = 0f; // Time since the last event occurred
    private float currentGrowth = 1f;
    private Transform plantTransform;

    private int randomEventsNumber = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hunger = startingHunger;
        plantTransform = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hunger = Mathf.Clamp(hunger - (Time.deltaTime * hungerDecayRate), 0f, maxHunger);
        Debug.Log("Hunger: " + hunger);

        // update UI or any other systems that depend on hunger value



        // event logic
        timeSinceLastEvent += Time.deltaTime;

        if (timeSinceLastEvent >= timeBetweenEvents)
        {
            // run event trigger logic

            TriggerEventChooser();
        }

        animator.SetFloat("Velocity", ((uint)agent.velocity.magnitude));

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 randomLocation;

            if (RandomPoint(transform.position, 10f, out randomLocation))
            {
                Debug.DrawRay(randomLocation, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(randomLocation);
            }
        }

    }
    
   void OnItemConsumed(float itemValue, GameObject itemObject) //Replace with the scriptable object and the item script
    {
        // Enum switch case for item types


        // shrink the item object to indicate it has been consumed
        // call an event on the item script to start this process, also take in speed to lerp
        

        // Increase hunger and decrease time between events

        increaseHunger(10f);

        // increase Growth based on the item value

        increaseGrowth(1.1f);

    }

    void increaseGrowth(float growthValue)
    {
        currentGrowth *= growthValue;
        plantTransform.localScale = new Vector3(currentGrowth, currentGrowth, currentGrowth);
        Debug.Log("Current Growth: " + currentGrowth);
    }

    void increaseHunger(float hungerValue)
    {
        hunger = Mathf.Clamp(hunger + hungerValue, 0f, maxHunger);
        // update UI or any other systems that depend on hunger value



        Debug.Log("Hunger: " + hunger);
    }

    void TriggerEventChooser()
    {
        
        // randomly choose an event

        int randomNumber = Random.Range(0, randomEventsNumber - 1);
        

        // Reset the timer
        timeSinceLastEvent = 0f;
        
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        if (NavMesh.SamplePosition(center + Random.insideUnitSphere * range, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
