using External.DI;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;
using UnityEngine.Events;

public class PostBox : MonoBehaviour, ILeftMouseDownClickable
{
    [SerializeField] private TV TV;
    [SerializeField] private TriggerObject triggerObject;
    [SerializeField] private Transform startPointObject;
    [SerializeField] private float timeBoxFallDown = 0.3f;
    [SerializeField] private float timeBoxFallUp = 1f;
    [SerializeField] private float timeLukeOpen = 0.5f;
    [SerializeField] private Animator lukeAnimator;

    private Player player;
    private ExerciseManager exerciseManager;
    private MovePointToPoint movePointToPoint;
    private ToastManager toastManager;

    private bool itemInBox = false;
    public Action OnPostBoxEmpty;
    private PickUpItem pickUpItemInBox;

    public List<string> nameRequirements;
    private string conditionExercise = "None";

    private bool isClick = false;
    private GameBootstrap gameBootstrap;
    public UnityEvent<PickUpItem> putObjectInBox;
    public UnityEvent<PickUpItem> pickUpObjectInBox;

    [SerializeField] private List<AudioClip> soundsLuke = new();
    [SerializeField] private List<AudioClip> soundsDropItem = new();
    [SerializeField] private List<AudioClip> soundsPickUpItem = new();
    [SerializeField] private List<AudioClip> soundsBoxDropDown = new();


    public void Init(Player player, ExerciseManager exerciseManager, ToastManager toastManager, GameBootstrap gameBootstrap)
    {
        this.gameBootstrap = gameBootstrap;

        TV.OnTVClose.AddListener(() =>
        {
            if (itemInBox)
            {
                CheckItemInBox(pickUpItemInBox);
            }
        });

        movePointToPoint = GetComponent<MovePointToPoint>();
        this.toastManager = toastManager;
        this.player = player;
        this.exerciseManager = exerciseManager;

        exerciseManager.PlayerGetExercise.AddListener((exercise) =>
        {
            nameRequirements.Clear();
            for (int i = 0; i < exercise.exerciseRequirements.Count; i++)
            {
                nameRequirements.Add(exercise.exerciseRequirements[i].typeRequirement);
            }
        });

        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                isClick = false;
                if (this.player.PlayerPickUpItem && itemInBox)
                {
                    gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                    gameBootstrap.OnPlayOneShotRandomSound(soundsPickUpItem);
                    PickUpItem playerPickUpItem = player.GetPickUpItem();
                    putObjectInBox?.Invoke(playerPickUpItem);
                    player.PutItem();
                    player.PickUpItem(PickUpItemInBox());
                    pickUpObjectInBox?.Invoke(player.GetPickUpItem());
                    PutItemInBox(playerPickUpItem);
                }
                else if (this.player.PlayerPickUpItem && itemInBox == false)
                {
                    gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                    putObjectInBox?.Invoke(player.GetPickUpItem());
                    PutItemInBox(player.GetPickUpItem());
                    player.PutItem();
                }
                else if (this.player.PlayerPickUpItem == false && itemInBox)
                {
                    gameBootstrap.OnPlayOneShotRandomSound(soundsPickUpItem);
                    player.PickUpItem(PickUpItemInBox());
                    pickUpObjectInBox?.Invoke(player.GetPickUpItem());
                }
            }
        });
    }

    public void PutItemInBox(PickUpItem pickUpItem)
    {
        pickUpItemInBox = pickUpItem;
        itemInBox = true;
        pickUpItemInBox.transform.parent = transform;
        pickUpItemInBox.transform.position = transform.position;

        CheckItemInBox(pickUpItem);

    }

    private void CheckItemInBox(PickUpItem pickUpItem)
    {
        for (int i = 0; i < nameRequirements.Count; i++)
        {
            if (pickUpItem.NameItem == nameRequirements[i])
            {
                //отправить имя полученного ингредиента ExerciseManager
                toastManager.ShowToast($"Отправлен объект: {nameRequirements[i]}");
                nameRequirements.Remove(nameRequirements[i]);
                StartCoroutine(WaitObjectFallDown(timeLukeOpen));

                if (nameRequirements.Count == 0)
                {
                    exerciseManager.CompleteExercise();
                }
                break;
            }
        }
    }

    private IEnumerator WaitObjectFallDown(float t)
    {
        lukeAnimator.StopPlayback();
        lukeAnimator.Play("LukeOpen");
        gameBootstrap.OnPlayOneShotRandomSound(soundsLuke);
        yield return new WaitForSeconds(t);
        movePointToPoint.StartMoveTo(timeBoxFallDown);
        yield return new WaitForSeconds(timeBoxFallDown + 0.5f);
        Destroy(pickUpItemInBox.gameObject);

        itemInBox = false;
        OnPostBoxEmpty?.Invoke();
        pickUpItemInBox = null;
        transform.position = startPointObject.position;
        movePointToPoint.StartMoveTo(timeBoxFallUp);
        yield return new WaitForSeconds(timeBoxFallUp);
        gameBootstrap.OnPlayOneShotRandomSound(soundsBoxDropDown);
    }

    public PickUpItem PickUpItemInBox()
    {
        itemInBox = false;
        OnPostBoxEmpty?.Invoke();
        PickUpItem temp = pickUpItemInBox;
        pickUpItemInBox = null;
        return temp;
    }

    public bool ItemInBox()
    {
        return itemInBox;
    }

    public void OnMouseLeftClickDownObject()
    {
        isClick = true;
    }

    public void OnMouseLeftClickDownOtherObject()
    {
        isClick = false;
    }

}
