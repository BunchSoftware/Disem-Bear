using External.DI;
using Game.Environment;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class OpenObject : MonoBehaviour, ILeftMouseClickable, IRightMouseClickable
{
    public float timeOpen = 1f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    public UnityEvent OnObjectOpen;
    public UnityEvent OnObjectClose;

    private bool isOpen = false;

    private CameraMoveToPosition lastMoveCameraToPosition;
    private Vector3 lastPlayerPosition;

    private MoveCamera moveCamera;
    private Player player;

    private PlayerMouseMove playerMouseMove;
    private TriggerObject triggerObject;

    private CameraMoveToPosition moveCameraToPosition = new();

    private bool isClick = false;


    public void Init(TriggerObject triggerObject, PlayerMouseMove playerMouseMove, Player player)
    {
        this.player = player;
        this.triggerObject = triggerObject;
        this.playerMouseMove = playerMouseMove;


        moveCamera = Camera.main.GetComponent<MoveCamera>();

        moveCameraToPosition = new CameraMoveToPosition()
        {
            time = timeOpen,
            position = cameraTransform.position,
            eulerAngles = cameraTransform.eulerAngles,
        };

        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick && isOpen == false && moveCamera.IsMove() == false)
            {
                isOpen = true;
                isClick = false;
                OnObjectOpen.Invoke();

                lastMoveCameraToPosition = new CameraMoveToPosition()
                {
                    time = timeOpen,
                    eulerAngles = moveCamera.transform.eulerAngles,
                    position = moveCamera.transform.position    
                };
                lastPlayerPosition = player.transform.position;

                moveCamera.StartMoveTo(moveCameraToPosition);
                playerMouseMove.StopPlayerMove();
                playerMouseMove.MovePlayer(playerTransform.position);
            }
        });
    }

    public void OnMouseLeftClickObject()
    {
        if (player.PlayerPickUpItem == false && !isOpen && moveCamera.IsMove() == false)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;
            pointerData.pointerId = -1;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.tag != "OpenObject") continue;

                isClick = true;
                break;
            }
        }
    }

    public void OnMouseRightClickObject()
    {
        if (isOpen && moveCamera.IsMove() == false)
        {
            moveCamera.StartMoveTo(lastMoveCameraToPosition);
            playerMouseMove.MovePlayer(lastPlayerPosition);
            isOpen = false;
            OnObjectClose.Invoke();
            playerMouseMove.ReturnPlayerMove();
        }
    }

    public void OnMouseRightClickOtherObject()
    {
        isClick = false;
    }
    public void OnMouseLeftClickOtherObject()
    {
        isClick = false;
    }
}
[Serializable]
public class CameraMoveToPosition
{
    public Vector3 position;
    public Vector3 eulerAngles;
    public float time;
}

