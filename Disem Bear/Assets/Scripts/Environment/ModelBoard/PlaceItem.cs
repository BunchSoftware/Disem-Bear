using External.DI;
using Game.Environment;
using Game.LPlayer;
using UnityEngine;

public class PlaceItem : MonoBehaviour, ILeftMouseDownClickable
{
    [SerializeField] private GameObject item;
    [SerializeField] private TriggerObject triggerObject;
    [SerializeField] private GameObject placeImage;
    [SerializeField] private AudioClip soundPutItem;
    [SerializeField] private string nameItem = "None";

    private ScaleChooseObject scaleChooseObject;

    private Player player;

    private bool isClick = false;

    public void Init(Player player, GameBootstrap gameBootstrap)
    {
        this.player = player;
        scaleChooseObject = GetComponent<ScaleChooseObject>();
        player.OnPickUpItem.AddListener((pickUpItem) =>
        {
            if (pickUpItem.NameItem == nameItem && placeImage != null)
            {
                placeImage.SetActive(true);
                scaleChooseObject.enabled = true;
            }
            else if (placeImage != null)
            {
                placeImage.SetActive(false);
                scaleChooseObject.enabled = false;
            }
        });
        player.OnPutItem.AddListener((putItem) =>
        {
            if (placeImage != null)
            {
                placeImage.SetActive(false);
                scaleChooseObject.enabled = false;
            }
        });
        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick)
            {
                if (this.player.PlayerPickUpItem)
                {
                    if (this.player.GetPickUpItem().NameItem == nameItem)
                    {
                        gameBootstrap.OnPlayOneShotSound(soundPutItem);
                        Destroy(this.player.GetPickUpItem().gameObject);
                        this.player.PutItem();
                        item.SetActive(true);
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
