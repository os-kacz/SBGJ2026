using System.Collections.Generic;
using NUnit.Framework;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlantLogic : MonoBehaviour
{

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip eatingSound;
    public AudioClip growlSound;

    // animation and AI
    private NavMeshAgent agent;
    private Animator animator;

    private PlantState plantState;

    // hunger variables
    [SerializeField]
    private float hunger  = 10f;
    [SerializeField]
    private float startingHunger = 10f;
    [SerializeField]
    private float maxHunger = 100f;
    [SerializeField]
    private float hungerDecayRate = 0.25f; // Hunger decay rate per second
    

    // item management variables
    [SerializeField]
    private float timeBetweenEvents = 10f; // Time between events in seconds
    [SerializeField]
    private float timeSinceLastEvent = 0f; // Time since the last event occurred
    private float currentGrowth = 0.25f;

    private bool hasEatItem = false;
    
    [SerializeField]
    private GameObject[] abilityObjects;
    [SerializeField]
    private bool[] abilityActives; 

    private Transform plantTransform;

    [SerializeField]
    public Transform mouthPosition;

    [SerializeField]
    private PlayerPickup playerPickup;

    private float MaxEatTime = 1.0f;

    private float currentEatTime = 0.0f;

    private bool shouldUpdate = true;

    private GameManager gameManager;

    [SerializeField]
    private Image hungerBar;

    private PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hunger = startingHunger;
        plantTransform = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        plantState = PlantState.Idle;
        gameManager = FindAnyObjectByType<GameManager>();
        abilityActives = new bool[abilityObjects.Length];
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        // update UI or any other systems that depend on hunger value
        
        if (shouldUpdate == false)
        {
            return;
        }

        // event logic
        timeSinceLastEvent += 1 * Time.deltaTime;

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
                PlayGrowlSound();

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

    void LateUpdate()
    {

        if (!shouldUpdate)
        {
            return;
        }

        if (hunger <= 0)
        {
            shouldUpdate = false;
            plantState = PlantState.Idle;
            gameManager.GameOver();
            animator.SetInteger("State", ((int)plantState));

            return;
        }
        else if (hunger >= maxHunger)
        {
            shouldUpdate = false;
            plantState = PlantState.Idle;
            gameManager.GameWin();
            animator.SetInteger("State", ((int)plantState));

            return;       
        }

        hunger -= hungerDecayRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, maxHunger + 5);
        hungerBar.fillAmount = hunger / maxHunger;

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
        PlayEatingSound();

        Debug.Log(itemScript.itemData.abilityType);
        // Enum switch case for item types
        switch (itemScript.itemData.abilityType)
        {
            case AbilityType.Fire:
                abilityActives[0] = true;
                break;
            case AbilityType.Electric:
                abilityActives[1] = true;
                break;
            case AbilityType.Explosive:
                abilityActives[2] = true;
                break;
            case AbilityType.Mist:
                abilityActives[3] = true;
                break;
        }

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

        timeBetweenEvents *= itemScript.itemData.abilityRate;

        if (!hasEatItem)
        {
            hasEatItem = true;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);
        }
    }

    void increaseGrowth(float growthValue)
    {
        currentGrowth *= growthValue;
        plantTransform.localScale = new Vector3(currentGrowth, currentGrowth, currentGrowth);
        Debug.Log("Current Growth: " + currentGrowth);
    }

    void increaseHunger(float hungerValue)
    {
        hunger = hunger + hungerValue;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger + 5.0f);
        // update UI or any other systems that depend on hunger value
        hungerBar.fillAmount = hunger / maxHunger;


        Debug.Log("Hunger: " + hunger);
    }

    void TriggerEventChooser()
    {
        
        // randomly choose an event

        if (hasEatItem && abilityObjects.Length != 0)
        {
            bool found = false;
            int loopProtect = 0;
            while (found == false && loopProtect < 10)
            {
                int randAbility = Random.Range(0, abilityObjects.Length);

                if (abilityActives[randAbility] == true)
                {
                    GameObject spawnedObject = Instantiate(abilityObjects[randAbility]);
                    spawnedObject.transform.position = transform.position;
                    found = true;
                    break;
                }
                loopProtect++;
            }
        }
        

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

    public void PlayEatingSound()
    {
        if (audioSource != null && eatingSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(eatingSound);
        }
    }

    public void PlayGrowlSound()
    {
        if (audioSource != null && growlSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(growlSound);
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
