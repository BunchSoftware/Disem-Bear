using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRoot : MonoBehaviour, IUpdateListener
{
    [Header("Transition Between Rooms")]
    [SerializeField] private List<NextRoom> nextRooms;
    [SerializeField] private GameObject invisibleWallBetweenRooms;

    public void Init(PlayerMouseMove playerMouseMove)
    {
        for (int i = 0; i < nextRooms.Count; i++)
        {
            nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
        }
    }
    public void OnUpdate(float deltaTime)
    {
        
    }
}
