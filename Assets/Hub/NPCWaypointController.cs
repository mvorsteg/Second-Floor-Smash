using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCWaypointController : MonoBehaviour
{
    private Queue<NPCWaypoint> availableWaypoints;
    private Dictionary<NPCControl, NPCWaypoint> ownedWaypoints;
    private Dictionary<string, NPCControl> npcNameLookup;

    private void Awake()
    {
        availableWaypoints = new Queue<NPCWaypoint>();
        ownedWaypoints = new Dictionary<NPCControl, NPCWaypoint>();
        npcNameLookup = new Dictionary<string, NPCControl>();
        
        NPCWaypoint[] waypointArr = GetComponentsInChildren<NPCWaypoint>();
        waypointArr.Shuffle();

        foreach (NPCWaypoint waypoint in waypointArr)
        {
            availableWaypoints.Enqueue(waypoint);
        }
        
    }

    public void CreateNPCNameLookup(IEnumerable<NPCControl> npcs)
    {
        foreach (NPCControl npc in npcs)
        {
            npcNameLookup[npc.tag] = npc;
        }
    }

    public NPCWaypoint GetNextWaypoint(NPCControl npc)
    {
        NPCWaypoint newWaypoint = null;

        // take old waypoint back from NPC and add to pool
        if (ownedWaypoints.TryGetValue(npc, out NPCWaypoint oldWaypoint) && oldWaypoint != null)
        {
            availableWaypoints.Enqueue(oldWaypoint);
        }

        // try to give NPC a new waypoint if any are available
        if (availableWaypoints.Count > 0)
        {
            newWaypoint = availableWaypoints.Dequeue();
        }
        ownedWaypoints[npc] = newWaypoint;
        return newWaypoint;

    }

    public bool IsOwnerInGroup(NPCWaypointGroup group)
    {
        // if group has no owner, no issues
        if (group.owners.Length <= 0)
        {
            return true;
        }
        // else check to see if owner is in the group
        foreach (string owner in group.owners)
        {
            if (npcNameLookup.TryGetValue(owner, out NPCControl npc))
            {
                if (ownedWaypoints.TryGetValue(npc, out NPCWaypoint waypoint))
                {
                    if (waypoint.group == group)
                    {
                        return true;
                    }
                }
            }
        }
        // else the owner is not here
        return false;
    }
}