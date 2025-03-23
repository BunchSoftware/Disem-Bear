using System;
using System.Collections;
using System.Collections.Generic;
using Game.Environment.LMixTable;
using Game.LDialog;
using UnityEngine;

public class CheckPutObjectPostBox : MonoBehaviour
{
    [SerializeField] private List<PutObjectPostBoxCondition> conditions = new();
    [SerializeField] private List<PutObjectPostBoxStartDialog> startDialogs = new();

    private PostBox postBox;

    public void Init(PostBox postBox, DialogManager dialogManager)
    {
        this.postBox = postBox;

        for (int i = 0; i < conditions.Count; i++)
        {
            PutObjectPostBoxCondition condition = conditions[i];
            postBox.putObjectInBox.AddListener((pickUpItem) =>
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
            PutObjectPostBoxStartDialog startDialog = startDialogs[i];
            postBox.putObjectInBox.AddListener((UnityEngine.Events.UnityAction<Game.Environment.Item.PickUpItem>)((pickUpItem) =>
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
    public class PutObjectPostBoxCondition
    {
        public int indexDialogPoint = -1;
        public int indexDialog = -1;
        public string nameItem = "None";
    }

    [Serializable]
    public class PutObjectPostBoxStartDialog
    {
        public string nameItem = "None";
        public int indexDialogPoint = 0;
        public bool on = false;
    }
}
