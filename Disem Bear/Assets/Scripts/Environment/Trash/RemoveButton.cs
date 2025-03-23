using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RemoveButton : MonoBehaviour, ILeftMouseDownClickable, IMouseOver
{
    private bool isClick = false;
    public bool isMouseOver = false;
    private TriggerObject triggerObject;
    [SerializeField] private Transform DownRemovePoint;


    public void Init(TriggerObject triggerObject, Player player, Trash trash, ToastManager toastManager)
    {
        this.triggerObject = triggerObject;
        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {
            if (isClick && player.PlayerPickUpItem == false)
            {
                isClick = false;
                if (trash.ItemInTrash())
                {
                    PickUpItem pickUpItemInTrash = trash.GetItemInTrash();
                    trash.PickUpItemInTrash();
                    toastManager.ShowToast(pickUpItemInTrash.NameItem + " выброшен в мусорку!");
                    pickUpItemInTrash.transform.DOMove(DownRemovePoint.position, 1f).SetEase(Ease.Linear);
                    pickUpItemInTrash.transform.DORotate(new Vector3(0, 0, 1440), 1f).SetEase(Ease.Linear);
                    pickUpItemInTrash.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 1f).SetEase(Ease.Linear);
                    StartCoroutine(DestroyItemInTrash(1f, pickUpItemInTrash.gameObject));
                }
                else
                {
                    toastManager.ShowToast("Ќечего выбрасывать!");
                }
            }
        });
    }

    IEnumerator DestroyItemInTrash(float t, GameObject gameObject)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }

    public void OnMouseEnterObject()
    {
        isMouseOver = true;
    }

    public void OnMouseExitObject()
    {
        isMouseOver = false;
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
