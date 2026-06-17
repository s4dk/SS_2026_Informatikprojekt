using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace NPC.Navigation
{
    
public class Area : MonoBehaviour
{
    [SerializeField]
    private SplineContainer spline;
    [SerializeField]
    private float radius = 3f;
    
    public float Raduis = 20f;
    private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Raduis);
        }

    public Vector3 GetRandomPoint()
        {
            float t = UnityEngine.Random.Range(0f, 1f);

            return spline.EvaluatePosition(t);
        }    
}
}