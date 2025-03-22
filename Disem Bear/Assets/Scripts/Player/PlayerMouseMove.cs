using External.DI;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        [SerializeField] private MakePathToObject makePathToObject;
        [SerializeField] private AudioClip steps;
        private GameBootstrap gameBootstrap;
        private MonoBehaviour context;
        private bool isCanMove = true;
        private bool isMoveNow = false;
        private const float TimeStep = 0.847f;
        private float timerStepDuration = TimeStep;
        private Move move = new Move();

        private Vector3 preveiusPosition;

        public void Init(MonoBehaviour context, NavMeshAgent navMeshAgent, GameBootstrap gameBootstrap)
        {
            this.context = context;
            this.navMeshAgent = navMeshAgent;
            this.gameBootstrap = gameBootstrap;
            move.transform = navMeshAgent.transform;
            preveiusPosition = move.transform.position;

            Debug.Log("PlayerMouseMove: ”ÒÔÂ¯ÌÓ ËÌËˆËÎËÁËÓ‚‡Ì");
        }


        public void OnUpdate(float deltaTime)
        {
            PlaySoundStep(deltaTime);
            if (isCanMove && Input.GetMouseButtonDown(0))
            {
                Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                float maxdistance = 100f;
                int layermask = -1;
                if (Physics.Raycast(movePosition, out var hitinfo, maxdistance, layermask, QueryTriggerInteraction.Ignore))
                {
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
                        return;
                    //if (makePathToObject.On)
                    //{
                    //    makePathToObject.gameBootstrap.OnPlayOneShotSound(makePathToObject.water); –¿— ŒÃ≈Õ“»“‹ ¬  –¿…Õ≈Ã —À”◊¿≈
                    //}
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
                isMoveNow = true;
            }
            else if (preveiusPosition.x > currentPosition.x
                 && horizontalPosition > verticalPosition)
            {
                move.directionMove = DirectionMove.Left;
                isMoveNow = true;
            }
            else if (preveiusPosition.z > currentPosition.z
                 && horizontalPosition < verticalPosition)
            {
                move.directionMove = DirectionMove.Forward;
                isMoveNow = true;
            }
            else if (preveiusPosition.z < currentPosition.z
                 && horizontalPosition < verticalPosition)
            {
                move.directionMove = DirectionMove.Back;
                isMoveNow = true;
            }
            else
            {
                move.directionMove = DirectionMove.State;
                isMoveNow = false;
            }

            OnMove(move);

            preveiusPosition = navMeshAgent.transform.position;
        }

        private void PlaySoundStep(float deltaTime)
        {
            if (isMoveNow)
            {
                if (timerStepDuration >= TimeStep)
                {
                    gameBootstrap.OnPlayOneShotSound(steps);
                    timerStepDuration = 0f;
                }
                timerStepDuration += deltaTime;
            }
            if (!isMoveNow && timerStepDuration < TimeStep)
            {
                timerStepDuration += deltaTime;
            }
        }

        public void MovePlayer(Vector3 position)
        {
            navMeshAgent.SetDestination(position);
        }
        public void StopPlayerMove()
        {
            isCanMove = false;
        }
        public void ReturnPlayerMove()
        {
            isCanMove = true;
        }
    }
}
