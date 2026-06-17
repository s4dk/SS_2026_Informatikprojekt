using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;
using Cinemachine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

namespace NPC.Navigation
{
    

public class NPCWander : NPCComponent
{
   
    public Area Area;

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

    [Header("Debugging")]
    [SerializeField]
    State state = State.Wandering;

    public void Start()
        {
            if(Random.Range(0f, 100.0f) > 50f)
            {
                ChangeState(State.Wandering);
            } 
            else ChangeState(State.Waiting);

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
            }
        }

    void ChangeState(State newState)
        {
           state = newState;

           if(state == State.Wandering)
            {
                npc.Agent.isStopped = false;

                SetRandomDestination();
            } 
            else if (state == State.Waiting)
            {
                waitTime = maxWaitTime + Random.Range(0f, maxWaitTimeRandom);

                npc.Agent.isStopped = true;
            }
        }

    bool hasArrived()
        {
            return npc.Agent.remainingDistance <= npc.Agent.stoppingDistance;
        }

   void SetRandomDestination()
        {
            npc.Agent.SetDestination(Area.GetRandomPoint());
        }

}
}