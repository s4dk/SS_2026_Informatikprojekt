using UnityEngine;

namespace NPC.Navigation
{
    public class NPCComponent : MonoBehaviour
    {
        protected NPC npc;

        protected virtual void Awake()
        {
            npc = GetComponentInParent<NPC>();
        }
    }

public class NPCAnimator : NPCComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        npc. Animator.SetFloat("Speed", npc.CurrentSpeed);
    }
}
}