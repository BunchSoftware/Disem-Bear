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
public class OpenObject : MonoBehaviour, IUpdateListener, ILeftMouseDownClickable
{
    public bool on = true;
    public float timeOpen = 1f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    public UnityEvent OnStartObjectOpen;
    public UnityEvent OnEndObjectOpen;

    public UnityEvent OnStartObjectClose;
    public UnityEvent OnEndObjectClose;

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
            if (isClick && isOpen == false && moveCamera.IsMove() == false && on)
            {
                isOpen = true;
                isClick = false;

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

                OnStartObjectOpen?.Invoke();

                StartCoroutine(IObjectOpen(timeOpen));
            }
        });
    }

    private IEnumerator IObjectOpen(float time)
    {
        yield return new WaitForSeconds(time);
        OnEndObjectOpen.Invoke();
    }

    private IEnumerator IObjectClose(float time)
    {
        yield return new WaitForSeconds(time);
        OnEndObjectClose.Invoke();
    }

    public void OnMouseLeftClickDownObject()
    {
        if (player.PlayerPickUpItem == false && !isOpen && moveCamera.IsMove() == false && on)
            isClick = true;
    }

    public void OnMouseLeftClickDownOtherObject()
    {
        isClick = false;
    }

    public void OnUpdate(float deltaTime)
    {
        if (Input.GetMouseButtonDown(1) && isOpen && moveCamera.IsMove() == false && on)
        {
            moveCamera.StartMoveTo(lastMoveCameraToPosition);
            playerMouseMove.MovePlayer(lastPlayerPosition);
            isOpen = false;
            playerMouseMove.ReturnPlayerMove();

            OnStartObjectClose?.Invoke();
            StartCoroutine(IObjectClose(timeOpen));
        }
    }
}
[Serializable]
public class CameraMoveToPosition
{
    public Vector3 position;
    public Vector3 eulerAngles;
    public float time;
}

