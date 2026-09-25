using UnityEngine;
using UnityEngine.AI;

public class StupidScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private PlantState plantState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetInteger("State", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector3 location;

                    if (RandomPoint(transform.position, 10f, out location))
                    {
                        Debug.DrawRay(location, Vector3.up, Color.blue, 1.0f);
                        agent.SetDestination(location);
                    }
                }
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
