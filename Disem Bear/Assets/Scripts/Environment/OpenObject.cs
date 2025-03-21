using External.DI;
using Game.Environment;
using Game.LPlayer;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OpenObject : MonoBehaviour, IUpdateListener, ILeftMouseDownClickable
{
    public bool on = true;
    public float timeOpen = 1f;
    public float timeWaitBeforeOpening = 0;
    public float timeClose = 1f;
    public float timeWaitBeforeClosing = 0;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private bool activePlayerInput = true;

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

                if (!activePlayerInput)
                    PlayerInput.isActive = false;
                OnStartObjectOpen?.Invoke();
                StartCoroutine(IObjectWaitBeforeOpening(timeWaitBeforeOpening));
            }
        });
    }

    private IEnumerator IObjectOpen(float time)
    {
        yield return new WaitForSeconds(time);
        OnEndObjectOpen.Invoke();
    }

    private IEnumerator IObjectWaitBeforeOpening(float time)
    {
        yield return new WaitForSeconds(time);

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

        player.EnterSomething(this);

        StartCoroutine(IObjectOpen(timeOpen));
    }

    private IEnumerator IObjectClose(float time)
    {
        yield return new WaitForSeconds(time);
        player.ExitSomething(this);
        if (!activePlayerInput)
            PlayerInput.isActive = true;
        OnEndObjectClose.Invoke();
    }

    private IEnumerator IObjectWaitBeforeClosing(float time)
    {
        yield return new WaitForSeconds(time);
        moveCamera.StartMoveTo(lastMoveCameraToPosition);
        playerMouseMove.MovePlayer(lastPlayerPosition);
        isOpen = false;
        playerMouseMove.ReturnPlayerMove();

        StartCoroutine(IObjectClose(timeClose));
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
            OnStartObjectClose?.Invoke();
            StartCoroutine(IObjectWaitBeforeClosing(timeWaitBeforeClosing));
        }
    }

    public void Open()
    {
        OnMouseLeftClickDownObject();
    }
    public void Close()
    {
        if (isOpen && moveCamera.IsMove() == false && on)
        {
            OnStartObjectClose?.Invoke();
            StartCoroutine(IObjectWaitBeforeClosing(timeWaitBeforeClosing));
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

