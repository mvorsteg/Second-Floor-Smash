using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCControl : MonoBehaviour
{
    public float roamingRadius = 10;
    public float idleTimerMax = 180f;
    public float idleTimerMin = 15f;

    private Animator anim;
    private NavMeshAgent nav;
    private Transform target;
    [SerializeField]
    private float timer;
    private float currentRoamTime;
    private bool isRoaming;

    // DEBUG and Temp stuff that should eventually be removed
    public NPCWaypoint destination; // leave this empty for regular behavior
    public NPCWaypointController waypointController;
    public bool startInRandomPosition = true;
    public float randomPositionStartingRadius = 120;
    
    
    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        timer = Random.Range(idleTimerMin, idleTimerMax);
        transform.Find("Body").localEulerAngles = new Vector3(0,0,0);
        StartCoroutine(Roaming());

        // Calculate the base offset adjustment using the capsule collider
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        nav.baseOffset = - (capsule.center.y - capsule.height / 2) - 0.05f;

        waypointController = FindObjectOfType<NPCWaypointController>();
        if (waypointController != null)
        {
            destination = waypointController.GetNextWaypoint(this);
            // go straight to this 1st dest
            nav.Warp(destination.transform.position);
            if (destination.action == NPCWaypointAction.Sitting)
            {
                anim.SetBool("Sit", true);
            }
        }
        // Remove this
        // Randomize starting position
        else
        {
            Vector3 initPos = GetNewDestination(transform.position, randomPositionStartingRadius, -1);
            transform.position = initPos;
        }
    }
 
    // Update is called once per frame
    void Update ()
    {        
        // Calculate angular velocity
        Vector3 s = transform.InverseTransformDirection(nav.velocity).normalized;
        float turn = s.x;

        // let the animator know what's going on
        anim.SetFloat("Speed", nav.velocity.magnitude);
        anim.SetFloat("Turn", turn * 2);

        if (isRoaming)
        {
            if (IsAtDestination())
            {
                isRoaming = false;
                if (destination.action == NPCWaypointAction.Sitting)
                {
                    anim.SetBool("Sit", true);
                }
                transform.rotation = destination.transform.rotation;
            }
        }
        // When time is up, roam to a new destination
        if (!isRoaming)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                // get new destination
                destination = waypointController.GetNextWaypoint(this);
                nav.SetDestination(destination.transform.position);
                timer = Random.Range(idleTimerMin, idleTimerMax);

                isRoaming = true;
                anim.SetBool("Sit", false);
            }
        }
    }

    IEnumerator Roaming()
    {
        while (true)
        {
            // Wait till we're at a door
            yield return new WaitUntil(() => nav.isOnOffMeshLink);

            OffMeshLinkData data = nav.currentOffMeshLinkData;

            // data.offMeshLink should never be null... and yet sometimes it is...
            if (data.offMeshLink != null)
            {
                Door door = data.offMeshLink.GetComponent<Door>();
                
                // Open door
                door.OpenDoor();
                
                // Face the correct direction
                transform.LookAt(data.endPos);

                // Wait until it is open
                yield return new WaitUntil(() => door.currentState == Door.State.Open);
                
                // Walk through the door
                yield return StartCoroutine(Walk(data));
                
                nav.CompleteOffMeshLink();
                // Close the door
                door.CloseDoor();
            }
            else // This shouldn't be needed but for some reason it (sometimes) is
            {
                Debug.LogWarning(gameObject.name + " encountered a null offMeshLink for no reason!");
                nav.CompleteOffMeshLink();
            }
        }
    }


    // Simply interpolate a straight line from start to end.
    IEnumerator Walk(OffMeshLinkData data)
    {
        float timeLeft = Vector3.Distance(data.offMeshLink.startTransform.position, data.offMeshLink.endTransform.position) * (1 / nav.speed) + 1;
        float passTime = timeLeft;

        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + nav.baseOffset * Vector3.up;

        Vector3 lookPos = endPos - startPos;
        lookPos.y = 0;

        while (timeLeft > 0)
        {
            transform.position = Vector3.Lerp(endPos, startPos, timeLeft / passTime);
            anim.SetFloat("Speed", nav.speed - 0.2f);

            timeLeft -= Time.deltaTime;

            yield return null;
        }
    }


    public static Vector3 GetNewDestination(Vector3 origin, float radius, int layermask) {
        // Get a random new destination
        Vector3 newDirection = Random.insideUnitSphere * radius;

        newDirection += origin;
        
        NavMeshHit navHit;
 
        NavMesh.SamplePosition(newDirection, out navHit, radius, layermask);
    
        return navHit.position;
    }

    private bool IsAtDestination()
    {
        // Check if we've reached the destination
        if (!nav.pathPending)
        {
            if (nav.remainingDistance <= nav.stoppingDistance)
            {
                if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
