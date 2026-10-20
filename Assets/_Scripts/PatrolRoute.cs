using System.Collections.Generic;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform waypointGroup;

    private readonly List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex;

    public bool HasWaypoints => waypoints.Count > 0;

    public Transform CurrentWaypoint
    {
        get
        {
            if (!HasWaypoints)
                return null;

            return waypoints[currentWaypointIndex];
        }
    }

    private void Awake()
    {
        LoadWaypoints();
    }

    private void LoadWaypoints()
    {
        waypoints.Clear();

        if (waypointGroup == null)
        {
            Debug.LogWarning(
                $"{name} mangler en Waypoint Group.",
                this);
            return;
        }

        foreach (Transform child in waypointGroup)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogWarning(
                $"{name}'s Waypoint Group har ingen børn.",
                this);
        }
    }

    public void GoToNextWaypoint()
    {
        if (!HasWaypoints)
            return;

        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Count)
            currentWaypointIndex = 0;
    }

    public void ResetRoute()
    {
        currentWaypointIndex = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (waypointGroup == null)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypointGroup.childCount; i++)
        {
            Transform current = waypointGroup.GetChild(i);
            Gizmos.DrawSphere(current.position, 0.2f);

            if (i + 1 < waypointGroup.childCount)
            {
                Transform next = waypointGroup.GetChild(i + 1);
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }

}
