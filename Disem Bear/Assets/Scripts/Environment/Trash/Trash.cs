using System.Collections;
using System.Collections.Generic;
using External.DI;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using UI;
using UnityEngine;

public class Trash : MonoBehaviour, ILeftMouseDownClickable, IMouseOver
{
    [SerializeField] private TriggerObject triggerObject;
    [SerializeField] private Transform pointPlaceItem;
    [SerializeField] private MovePointToPoint removeButtonMove;
    [SerializeField] private float timeRemoveButtonMove = 0.3f;
    [SerializeField] private RemoveButton removeButton;
    private string stateRemoveButton = "down";
    private float timerToDownRemoveButton = 0f;
    private Player player;
    private ToastManager toastManager;
    private ToolBase toolBase;

    private bool itemInTrash = false;
    private PickUpItem pickUpItemInTrash;
    private bool isClick = false;

    [SerializeField] private List<AudioClip> soundsDropItem = new();
    [SerializeField] private List<AudioClip> soundsPickUpItem = new();


    public void Init(Player player, GameBootstrap gameBootstrap, ToastManager toastManager)
    {
        toolBase = GetComponent<ToolBase>();
        this.toastManager = toastManager;
        this.player = player;
        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                isClick = false;
                if (this.player.PlayerPickUpItem && itemInTrash)
                {
                    if (player.GetPickUpItem().questItem == false)
                    {
                        gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                        gameBootstrap.OnPlayOneShotRandomSound(soundsPickUpItem);
                        PickUpItem playerPickUpItem = player.GetPickUpItem();
                        player.PutItem();
                        player.PickUpItem(PickUpItemInTrash());
                        PutItemInTrash(playerPickUpItem);
                    }
                    else
                    {
                        toastManager.ShowToast("Не стоит это выбрасывать!");
                    }
                }
                else if (this.player.PlayerPickUpItem && itemInTrash == false)
                {
                    if (player.GetPickUpItem().questItem == false)
                    {
                        gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                        PutItemInTrash(player.GetPickUpItem());
                        player.PutItem();
                    }
                    else
                    {
                        toastManager.ShowToast("Не стоит это выбрасывать!");
                    }
                }
                else if (this.player.PlayerPickUpItem == false && itemInTrash)
                {
                    gameBootstrap.OnPlayOneShotRandomSound(soundsPickUpItem);
                    player.PickUpItem(PickUpItemInTrash());
                }
            }
        });

        if (itemInTrash)
        {
            toolBase.on = true;
            toolBase.toolTipText = pickUpItemInTrash.NameItem;
        }
        else
        {
            toolBase.on = false;
        }
        removeButton.Init(triggerObject, player, this, toastManager);
    }

    public void OnUpdate(float deltaTime)
    {
        if (timerToDownRemoveButton < 2f)
        {
            timerToDownRemoveButton += deltaTime;
        }
        switch (stateRemoveButton)
        {
            case "up":
                if (removeButtonMove.GetCurrentState() == "point1")
                {
                    removeButtonMove.StartMoveTo(timeRemoveButtonMove);
                }
                break;
            case "down":
                if (removeButtonMove.GetCurrentState() == "point2" && timerToDownRemoveButton > 1f && removeButton.isMouseOver == false)
                {
                    removeButtonMove.StartMoveTo(timeRemoveButtonMove);
                }
                break;
        }
    }

    private void PutItemInTrash(PickUpItem pickUpItem)
    {
        pickUpItemInTrash = pickUpItem;
        itemInTrash = true;
        pickUpItemInTrash.transform.parent = transform;
        pickUpItemInTrash.transform.position = pointPlaceItem.position;

        toolBase.on = true;
        toolBase.toolTipText = pickUpItemInTrash.NameItem;
        toolBase.OnToolTip();
    }

    public PickUpItem PickUpItemInTrash()
    {
        itemInTrash = false;
        PickUpItem temp = pickUpItemInTrash;
        pickUpItemInTrash = null;
        toolBase.on = false;
        toolBase.OffToolTip();
        return temp;
    }

    public bool ItemInTrash()
    {
        return itemInTrash;
    }

    public PickUpItem GetItemInTrash()
    {
        return pickUpItemInTrash;
    }

    public void OnMouseLeftClickDownObject()
    {
        isClick = true;
    }

    public void OnMouseLeftClickDownOtherObject()
    {
        isClick = false;
    }

    public void OnMouseEnterObject()
    {
        stateRemoveButton = "up";
    }

    public void OnMouseExitObject()
    {
        stateRemoveButton = "down";
        timerToDownRemoveButton = 0;
    }
}
