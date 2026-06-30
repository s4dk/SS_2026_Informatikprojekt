using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NPC.Navigation
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]

public class NPC : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [HideInInspector]
    public NavMeshAgent Agent;

    [HideInInspector]
    public Animator Animator;

    private Vector3 lastPosition;
    private float currentSpeed;

    public float CurrentSpeed
        {
            get{return Agent.velocity.magnitude;}
        }
    
    private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            lastPosition = transform.position; 
        }

    private void Update()
    {
    currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

    lastPosition = transform.position;
    }

}
}