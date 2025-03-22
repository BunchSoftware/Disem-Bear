using System.Collections;
using System.Collections.Generic;
using External.DI;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using UI;
using UnityEngine;

public class Trash : MonoBehaviour, ILeftMouseDownClickable
{
    [SerializeField] private TriggerObject triggerObject;
    [SerializeField] private Transform pointPlaceItem;
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
                    gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                    gameBootstrap.OnPlayOneShotRandomSound(soundsPickUpItem);
                    PickUpItem playerPickUpItem = player.GetPickUpItem();
                    player.PutItem();
                    player.PickUpItem(PickUpItemInTrash());
                    PutItemInTrash(playerPickUpItem);
                }
                else if (this.player.PlayerPickUpItem && itemInTrash == false)
                {
                    gameBootstrap.OnPlayOneShotRandomSound(soundsDropItem);
                    if (player.GetPickUpItem().questItem == false)
                    {
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

    private PickUpItem PickUpItemInTrash()
    {
        itemInTrash = false;
        PickUpItem temp = pickUpItemInTrash;
        pickUpItemInTrash = null;
        toolBase.on = false;
        toolBase.OffToolTip();
        return temp;
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
