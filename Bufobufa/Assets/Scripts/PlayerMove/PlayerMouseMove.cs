using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMouseMove : MonoBehaviour
{
    //LayerMask layerMask = LayerMask.GetMask("Player", "water");
    private NavMeshAgent agent;
    public bool MoveOn = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(movePosition, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Floor")))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }
}
