using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NPC.Navigation
{

public class NPCGenerator : MonoBehaviour
{
    [SerializeField]
    NPC NPCPrefab;
    [SerializeField]
    Area Area;

    [SerializeField]
    int count = 3;

        private void Start()
        {
            for(int i = 0; i < count; i++)
            {
                Vector3 position = Area.GetRandomPoint();
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                NPC npc = Instantiate(NPCPrefab, position, rotation);
                npc.GetComponent<NPCWander>().Area = Area;
            }
        }

    }

}