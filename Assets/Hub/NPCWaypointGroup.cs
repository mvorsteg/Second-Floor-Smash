using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWaypointGroup : MonoBehaviour
{
    [TagSelector]
    public string[] owners;
    [HideInInspector]
    public NPCWaypoint[] waypoints;

    private void Start()
    {
        waypoints = GetComponentsInChildren<NPCWaypoint>();
        foreach (NPCWaypoint waypoint in waypoints)
        {
            waypoint.group = this;
        }
    }
}