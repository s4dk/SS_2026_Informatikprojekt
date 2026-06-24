using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;
using Cinemachine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.Splines;

namespace NPC.Navigation
{
    

public class NPCWander : NPCComponent
{
   
    public Area Area;
    public Spline spline;
    public SplineContainer splineContainer;

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
    float currentMaxWaitTime = 3f;

    [SerializeField]
    private float waitTime = 0f;

    [SerializeField] private int waypointCount = 20;
    [SerializeField]
    private float speed = 0.1f;

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
            
            if(Random.Range(0f, 100.0f) > 50f)
            {
                ChangeState(State.Wandering);
            } 
            else ChangeState(State.Waiting);
            
        }

    void GenerateWaypoints()
    {
    waypoints.Clear();

        for (int i = 0; i < waypointCount; i++)
        {
            float t = i / (float)(waypointCount - 1);

            Vector3 point = splineContainer.EvaluatePosition(t);

                if (NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    waypoints.Add(hit.position);
                }
        }
    }

    private void Update()
        {
        
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
                transform.position = splineContainer.EvaluatePosition(t);
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
                waitTime = maxWaitTime + Random.Range(0f, maxWaitTimeRandom);
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