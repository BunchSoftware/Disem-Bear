using Game.LDialog;
using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class PointerTutorialManager : MonoBehaviour
    {
        private DialogManager dialogManager;
        [SerializeField] private MakePathToObject makePathToObject;

        [SerializeField] private List<Pointer> pointers = new();

        public void Init(DialogManager dialogManager, Player player)
        {
            makePathToObject.Init(player);

            this.dialogManager = dialogManager;
            this.dialogManager.OnStartDialog.AddListener(SetPointerStartDialog);
            this.dialogManager.OnFullEndDialog.AddListener(SetPointerFullEndDialog);

        }

        public void OnUpdate(float deltaTime)
        {
            makePathToObject.OnUpdate(deltaTime);
        }

        public void SetPointerStartDialog(Dialog dialog)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && pointers[i].indexDialog == dialogManager.GetCurrentIndexDialog())
                {
                    if (pointers[i].needTargetFullEndDialog == false)
                    {
                        if (pointers[i].offPointer)
                        {
                            makePathToObject.ClearTarget();
                        }
                        else
                        {
                            makePathToObject.SetTarget(pointers[i].lineToObjectTransform);
                        }
                        return;
                    }
                }
            }
            makePathToObject.ClearTarget();
        }

        public void SetPointerFullEndDialog(Dialog dialog)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i].indexDialogPoint == dialogManager.GetCurrentIndexDialogPoint() && pointers[i].indexDialog == dialogManager.GetCurrentIndexDialog())
                {
                    if (pointers[i].needTargetFullEndDialog)
                    {
                        if (pointers[i].offPointer)
                        {
                            makePathToObject.ClearTarget();
                        }
                        else
                        {
                            makePathToObject.SetTarget(pointers[i].lineToObjectTransform);
                        }
                        return;
                    }
                }
            }
            makePathToObject.ClearTarget();
        }

        [Serializable]
        public class Pointer
        {
            public int indexDialogPoint = 0;
            public int indexDialog = 0;
            [Space]
            public bool needTargetFullEndDialog = false;
            [Space]
            [Space]
            public bool offPointer = false;
            [Space]
            public Transform lineToObjectTransform;
        }
    }
}
