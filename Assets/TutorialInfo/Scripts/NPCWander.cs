using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace NPC.Navigation
{
    

public class NPCWander : NPCComponent
{
   
    public Area Area;
    public Spline spline;
    public SplineContainer splineContainer;

    public SplineContainer pendingSplineContainer;

    //finite state machine for wandering behaviour
    enum State
    {
        Wandering,
        Waiting

    }
    [SerializeField]
    float maxWaitTime = 3f;
    [SerializeField]
    float maxWaitTimeRandom = 5f;
    
    [SerializeField]
    private float waitTime = 0f;

    [SerializeField] private int waypointCount = 20;
    [SerializeField]
    private float speed = 0.1f;
    [SerializeField] private float detectionRadius = 5f;
    private float t = 0f;

    private List<Vector3> waypoints = new();
    private int currentWaypoint = 0;

    [Header("Debugging")]
    [SerializeField]
    State state = State.Wandering;

    public void Start()
        {
            
            npc.Agent.enabled = false;
            //GenerateWaypoints();

            //currentWaypoint = Random.Range(0, waypoints.Count);
            
            if(UnityEngine.Random.Range(0f, 100.0f) > 50f)
            {
                ChangeState(State.Wandering);
            } 
            else ChangeState(State.Waiting);
            
        }


    private void OnEnable()
    {
        SplineRegistry.OnSplineCreated += HandleNewSpline;
    }

    private void OnDisable()
    {
        SplineRegistry.OnSplineCreated -= HandleNewSpline;
    }

    private void HandleNewSpline(SplineContainer newSpline)
    {
    float3 nearest;
    float newT;
    float dist = SplineUtility.GetNearestPoint(
        newSpline.Spline,
        (float3)transform.position,
        out nearest,
        out newT
    );

    // nur reagieren, wenn der neue Spline nah genug am NPC ist
        if (dist <= detectionRadius)
        {
            pendingSplineContainer = newSpline;
        }
    }

    private void CheckForSplineSwitch()
    {
        if (pendingSplineContainer == null) return;

        float3 nearest;
        float newT;
        SplineUtility.GetNearestPoint(
            pendingSplineContainer.Spline,
            (float3)transform.position,
            out nearest,
            out newT
        );

        splineContainer = pendingSplineContainer;
        t = newT;
        pendingSplineContainer = null;
    }

    private void Update()
        {
        
            CheckForSplineSwitch();

            if(state == State.Waiting)
            {
                waitTime -= Time.deltaTime;
                if (waitTime < 0f)
                {
                    ChangeState(State.Wandering);
                }
            }
            else if (state == State.Wandering)
            {
             if(hasArrived()) ChangeState(State.Waiting);
             t += speed * Time.deltaTime;
             if(t > 1f)
                {
                    t -= 1;
                }   
                
                Vector3 position = splineContainer.EvaluatePosition(t);
                Vector3 forward = splineContainer.EvaluateTangent(t);

                transform.position = position;

                if(forward.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(forward);
                }
            }
        }

    void ChangeState(State newState)
        {
           state = newState;

           if(state == State.Wandering)
            {
                npc.Agent.isStopped = false;
                //SetNextWaypoint();
                //SetRandomDestination();
            } 
            else if (state == State.Waiting)
            {
                npc.Agent.isStopped = true;
                waitTime = maxWaitTime + UnityEngine.Random.Range(0f, maxWaitTimeRandom);
            }
        }

    bool hasArrived()
        {
            return npc.Agent.remainingDistance <= npc.Agent.stoppingDistance;
        }

   void SetRandomDestination()
        {
            npc.Agent.SetDestination(Area.GetRandomPoint());

             npc.Agent.SetDestination(waypoints[currentWaypoint]);

            currentWaypoint++;

            if(currentWaypoint >= waypoints.Count)
                currentWaypoint = 0; // loop
        }

    void SetNextWaypoint()
    {
    if (waypoints.Count == 0)
        return;

    npc.Agent.SetDestination(waypoints[currentWaypoint]);

    currentWaypoint++;

    if (currentWaypoint >= waypoints.Count)
        currentWaypoint = 0;
    }   
}
}