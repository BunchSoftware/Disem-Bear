using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMouseMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool MoveOn = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if(MoveOn && Input.GetMouseButtonDown(0))
        {
            Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(movePosition, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Floor", "ClickedObject")))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                agent.SetDestination(hitInfo.point);
            }
        }
    }
    public void MovePlayer(Vector3 pos)
    {
        agent.SetDestination(pos);
    }
    public void StopPlayerMove()
    {
        MoveOn = false;
    }
    public void ReturnPlayerMove()
    {
        MoveOn = true;
    }
}
