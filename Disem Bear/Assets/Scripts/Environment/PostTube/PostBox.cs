using System.Collections;
using System.Collections.Generic;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using UnityEngine;

public class PostBox : MonoBehaviour, ILeftMouseDownClickable
{
    [SerializeField] private TriggerObject triggerObject;
    private Player player;

    private bool itemInBox = false;
    private PickUpItem pickUpItemInBox;

    private bool isClick = false;


    public void Init(Player player)
    {
        this.player = player;

        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                isClick = false;
                if (this.player.PlayerPickUpItem && itemInBox == false)
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
    }

    public void PutItemInBox(PickUpItem pickUpItem)
    {
        pickUpItemInBox = pickUpItem;
        itemInBox = true;
        pickUpItemInBox.transform.parent = transform;
        pickUpItemInBox.transform.position = transform.position;
        
        
    }

    public PickUpItem PickUpItemInBox()
    {
        itemInBox = false;
        return pickUpItemInBox;
    }

    public bool ItemInbox()
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
