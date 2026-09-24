using UnityEngine;
using UnityEngine.AI;

public class PlantLogic : MonoBehaviour
{

    // animation and AI
    private NavMeshAgent agent;
    private Animator animator;

    private PlantState plantState;

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
    private float currentGrowth = 0.25f;
    private Transform plantTransform;

    [SerializeField]
    public Transform mouthPosition;

    private int randomEventsNumber = 5;

    [SerializeField]
    private PlayerPickup playerPickup;

    private float MaxEatTime = 1.0f;

    private float currentEatTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hunger = startingHunger;
        plantTransform = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        plantState = PlantState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        hunger = Mathf.Clamp(hunger - (Time.deltaTime * hungerDecayRate), 0f, maxHunger);

        // update UI or any other systems that depend on hunger value



        // event logic
        timeSinceLastEvent += Time.deltaTime;

        if (timeSinceLastEvent >= timeBetweenEvents)
        {
            // run event trigger logic

            TriggerEventChooser();
        }


        // plant AI states

        switch (plantState)
        {
            case PlantState.Idle:
                Vector3 randomLocation;

                if (RandomPoint(transform.position, 10f, out randomLocation))
                {
                    agent.SetDestination(randomLocation);
                    plantState = PlantState.Walking;
                }

                break;

            case PlantState.Walking:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector3 location;

                    if (RandomPoint(transform.position, 10f, out location))
                    {
                        Debug.DrawRay(location, Vector3.up, Color.blue, 1.0f);
                        agent.SetDestination(location);
                    }
                }
                break;
            
            case PlantState.Waiting:
                agent.SetDestination(gameObject.transform.position);

                break;

            case PlantState.Eating:
                agent.SetDestination(gameObject.transform.position);

                currentEatTime += 0.75f * Time.deltaTime;

                if (currentEatTime >= MaxEatTime)
                {
                    currentEatTime = 0.0f;
                    plantState = PlantState.Idle;
                }
                break;
        }

        animator.SetInteger("State", ((int)plantState));

        animator.SetFloat("Velocity", ((uint)agent.velocity.magnitude));

    }
    
   public void OnItemConsumed(ItemPickup itemScript, GameObject itemObject) //Replace with the scriptable object and the item script
    {

        if (playerPickup == null || itemScript == null)
        {
            return;
        }

        if (itemScript.beingConsumed)
        {
            return;
        }

        Debug.Log("Eating");
        plantState = PlantState.Eating;

        // Enum switch case for item types


        // shrink the item object to indicate it has been consumed
        // call an event on the item script to start this process, also take in speed to lerp

        if (playerPickup.GetHeldItemData() == null || playerPickup.GetHeldItemData() != itemObject)
        {
            
        }
        else
        {
            playerPickup.DropItem();
        }
        
        itemScript.OnConsumed(1f, mouthPosition.position);
        
        // Increase hunger and decrease time between events

        increaseHunger(itemScript.itemData.hungerRate);

        // increase Growth based on the item value

        increaseGrowth(itemScript.itemData.growthRate);

        timeBetweenEvents = itemScript.itemData.abilityRate;

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


    public void OpenMouth()
    {
        if (plantState != PlantState.Eating || plantState != PlantState.Waiting)
        {
            agent.SetDestination(gameObject.transform.position);
            plantState = PlantState.Waiting;
        }
    }

    public void CloseMouth()
    {
        if (plantState != PlantState.Eating)
        {
            plantState = PlantState.Idle;
        }
    }
}

public enum PlantState
{
    Idle,
    Walking,
    Waiting,
    Eating
}
