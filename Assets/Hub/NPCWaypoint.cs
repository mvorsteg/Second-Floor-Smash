using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCWaypointAction
{
    Standing,
    Sitting,
    Studying,
    Guitar,
}

public class NPCWaypoint : MonoBehaviour
{
    public NPCWaypointAction action;
    public Color color;
    [HideInInspector]
    public NPCWaypointGroup group;

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 0.4f);
        Gizmos.DrawRay(transform.position, transform.forward);       
    }
}