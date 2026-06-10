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

    public float CurrentSpeed
        {
            get{return Agent.velocity.magnitude;}
        }
    
    private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
        }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}