using System.Collections;
using System.Collections.Generic;
using Game.Environment;
using Game.LPlayer;
using UnityEngine;

public class PlaceBoard : MonoBehaviour, ILeftMouseDownClickable
{
    [SerializeField] private GameObject board;
    [SerializeField] private TriggerObject triggerObject;

    private Player player;

    private bool isClick = false;

    public void Init(Player player)
    {
        this.player = player;
        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                if (this.player.PlayerPickUpItem)
                {
                    if (this.player.GetPickUpItem().NameItem == "ModelBoard")
                    {
                        Destroy(this.player.GetPickUpItem().gameObject);
                        this.player.PutItem();
                        board.SetActive(true);
                        Destroy(gameObject);
                    }
                }
            }
        });
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
