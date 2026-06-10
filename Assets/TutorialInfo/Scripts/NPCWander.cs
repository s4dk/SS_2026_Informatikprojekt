using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;

namespace NPC.Navigation
{
    

public class NPCWander : NPCComponent
{
    [SerializeField]
    public Area Area;

    public void Start()
        {
            SetRandomDestination();
        }

    private void Update()
        {
            if(hasArrived())
            {
                SetRandomDestination();
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