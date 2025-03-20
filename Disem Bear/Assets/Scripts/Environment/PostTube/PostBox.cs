using System;
using System.Collections;
using System.Collections.Generic;
using External.DI;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;

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
    private ToolBase toolBase;

    private bool itemInBox = false;
    public Action OnPostBoxEmpty;
    private PickUpItem pickUpItemInBox;

    public List<string> nameRequirements;
    private string conditionExercise = "None";

    private bool isClick = false;
    private GameBootstrap gameBootstrap;

    [Header("soundsLuke")]
    [SerializeField] private List<AudioClip> soundsLuke = new();


    public void Init(Player player, ExerciseManager exerciseManager, ToastManager toastManager, GameBootstrap gameBootstrap)
    {
        this.gameBootstrap = gameBootstrap;
        toolBase = GetComponent<ToolBase>();

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
            conditionExercise = exercise.conditionExercise;
        });

        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                isClick = false;
                if (this.player.PlayerPickUpItem && itemInBox)
                {
                    PickUpItem playerPickUpItem = player.GetPickUpItem();
                    player.PutItem();
                    player.PickUpItem(PickUpItemInBox());
                    PutItemInBox(playerPickUpItem);
                }
                else if (this.player.PlayerPickUpItem && itemInBox == false)
                {
                    PutItemInBox(player.GetPickUpItem());
                    player.PutItem();
                }
                else if (this.player.PlayerPickUpItem == false && itemInBox)
                {
                    player.PickUpItem(PickUpItemInBox());
                }
            }
        });


        if (itemInBox)
        {
            toolBase.on = true;
            toolBase.toolTipText = pickUpItemInBox.NameItem;
        }
        else
        {
            toolBase.on = false;
        }
    }

    public void PutItemInBox(PickUpItem pickUpItem)
    {
        pickUpItemInBox = pickUpItem;
        itemInBox = true;
        pickUpItemInBox.transform.parent = transform;
        pickUpItemInBox.transform.position = transform.position;

        toolBase.on = true;
        toolBase.toolTipText = pickUpItemInBox.NameItem;
        toolBase.OnToolTip();

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
                    exerciseManager.DoneCurrentExercise(conditionExercise);
                }
                break;
            }
        }
    }

    IEnumerator WaitObjectFallDown(float t)
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
    }

    public PickUpItem PickUpItemInBox()
    {
        itemInBox = false;
        OnPostBoxEmpty?.Invoke();
        PickUpItem temp = pickUpItemInBox;
        pickUpItemInBox = null;
        toolBase.on = false;
        toolBase.OffToolTip();
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
