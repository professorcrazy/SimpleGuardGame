using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardMovement : MonoBehaviour
{
    [SerializeField] private Transform waypointGroup;
    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent agent;
    private Vector3 targetPos = Vector3.zero;

    public void SetTargetPosition(Vector3 position)
    {
        targetPos = position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waypointGroup != null)
        {
            foreach (Transform child in waypointGroup)//.GetComponentInChildren<Transform>()
            {
                waypoints.Add(child);
            }
        }
        agent = GetComponent<NavMeshAgent>();
        SetTargetPosition(waypoints[0].position);

    }
    public void NextWaypoint()
    {
        if (agent.remainingDistance < 0.1f)
            SetTargetPosition(waypoints[Random.Range(0, waypoints.Count)].position);
    }

    // FixedUpdate is called every physics update
    void FixedUpdate()
    {
        agent.SetDestination(targetPos);
        
    }
}
