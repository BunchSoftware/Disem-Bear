using External.DI;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.LPlayer
{
    public enum DirectionMove
    {
        State = 0, Right = 1, Left = 2, Forward = 3, Back = 4,
    }

    public class Move
    {
        public Transform transform;
        public DirectionMove directionMove = DirectionMove.State;
    }

    [Serializable]
    public class PlayerMouseMove : IUpdateListener
    {
        [HideInInspector] public Action<Move> OnMove;
        [SerializeField] private NavMeshAgent navMeshAgent;
        private bool isMove = true;
        private Move move = new Move();

        private Vector3 preveiusPosition;

        public void Init(NavMeshAgent navMeshAgent)
        {
            this.navMeshAgent = navMeshAgent;
            move.transform = navMeshAgent.transform;
            preveiusPosition = move.transform.position;

            Debug.Log("PlayerMouseMove: Успешно иницилизирован");
        }

        public void OnUpdate(float deltaTime)
        {
            if (isMove && Input.GetMouseButtonDown(0))
            {
                Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                float maxdistance = 100f;
                int layermask = -1;
                if (Physics.Raycast(movePosition, out var hitinfo, maxdistance, layermask, QueryTriggerInteraction.Ignore))
                {
                    if (EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>())
                        return;

                    MovePlayer(hitinfo.point);
                }
            }

            Vector3 currentPosition = navMeshAgent.transform.position;

            float horizontalPosition = Mathf.Abs(preveiusPosition.x - currentPosition.x);
            float verticalPosition = Mathf.Abs(preveiusPosition.z - currentPosition.z);

            move.transform = navMeshAgent.transform;

            if (preveiusPosition.x < currentPosition.x
                && horizontalPosition > verticalPosition)
            {
                move.directionMove = DirectionMove.Right;
            }
            else if (preveiusPosition.x > currentPosition.x
                 && horizontalPosition > verticalPosition)
            {
                move.directionMove = DirectionMove.Left;
            }
            else if (preveiusPosition.z > currentPosition.z
                 && horizontalPosition < verticalPosition)
            {
                move.directionMove = DirectionMove.Forward;
            }
            else if (preveiusPosition.z < currentPosition.z
                 && horizontalPosition < verticalPosition)
            {
                move.directionMove = DirectionMove.Back;
            }
            else
            {
                move.directionMove = DirectionMove.State;
            }

            OnMove(move);

            preveiusPosition = navMeshAgent.transform.position;
        }

        public void MovePlayer(Vector3 position)
        {
            navMeshAgent.SetDestination(position);
        }
        public void StopPlayerMove()
        {
            isMove = false;
        }
        public void ReturnPlayerMove()
        {
            isMove = true;
        }
    }
}
