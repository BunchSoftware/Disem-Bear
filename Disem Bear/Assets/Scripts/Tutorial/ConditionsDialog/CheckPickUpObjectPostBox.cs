using System;
using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UnityEngine;

public class CheckPickUpObjectPostBox : MonoBehaviour
{
    [SerializeField] private List<PickUpObjectPostBoxCondition> conditions = new();
    [SerializeField] private List<PickUpObjectPostBoxStartDialog> startDialogs = new();

    private PostBox postBox;

    public void Init(PostBox postBox, DialogManager dialogManager)
    {
        this.postBox = postBox;

        for (int i = 0; i < conditions.Count; i++)
        {
            PickUpObjectPostBoxCondition condition = conditions[i];
            postBox.pickUpObjectInBox.AddListener((pickUpItem) =>
            {
                if (dialogManager.GetCurrentIndexDialogPoint() == condition.indexDialogPoint && dialogManager.GetCurrentIndexDialog() == condition.indexDialog &&
                pickUpItem.NameItem == condition.nameItem)
                {
                    dialogManager.SkipReplica();
                }
            });
        }
        for (int i = 0; i < startDialogs.Count; i++)
        {
            PickUpObjectPostBoxStartDialog startDialog = startDialogs[i];
            postBox.pickUpObjectInBox.AddListener((UnityEngine.Events.UnityAction<Game.Environment.Item.PickUpItem>)((pickUpItem) =>
            {
                if (dialogManager.IsDialogOn() == false && startDialog.on)
                {
                    dialogManager.StartDialog(startDialog.indexDialogPoint);
                    startDialog.on = false;
                }
            }));
        }
    }


    [Serializable]
    public class PickUpObjectPostBoxCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public string nameItem = "None";
    }

    [Serializable]
    public class PickUpObjectPostBoxStartDialog
    {
        public string nameItem = "None";
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
