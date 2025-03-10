using System;
using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UnityEngine;

public class BlockTutorialManager : MonoBehaviour
{
    //Функция для того, чтобы ставить блоки на объектах, дабы с ними было невозможно взаимодействие
    //При добавлении нового блока нужно прописать его в 6 местах скрипта


    [SerializeField] private BlockObject WorkbenchBlock;
    [SerializeField] private BlockObject AquariumBlock;
    [SerializeField] private BlockObject FridgeBlock;
    [SerializeField] private BlockObject PrinterBlock;
    [SerializeField] private BlockObject TVBlock;
    [SerializeField] private BlockObject ModelBoardBlock;
    //Тут1

    [SerializeField] private List<BoolBlockObjects> tutorialBlocks = new();

    private DialogManager dialogManager;

    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;
        this.dialogManager.OnStartDialog.AddListener(SetBlockStartDialog);
        this.dialogManager.OnFullEndDialog.AddListener(SetBlockFullEndDialog);

        WorkbenchBlock.Init();
        AquariumBlock.Init();
        FridgeBlock.Init();
        PrinterBlock.Init();
        TVBlock.Init();
        ModelBoardBlock.Init();
        //Тут2
    }

    public void OnUpdate(float deltaTime)
    {
        WorkbenchBlock.OnUpdate(deltaTime);
        AquariumBlock.OnUpdate(deltaTime);
        FridgeBlock.OnUpdate(deltaTime);
        PrinterBlock.OnUpdate(deltaTime);
        TVBlock.OnUpdate(deltaTime);
        ModelBoardBlock.OnUpdate(deltaTime);
        //Тут3
    }

    public void SetBlockStartDialog(Dialog dialog)
    {
        for (int i = 0; i < tutorialBlocks.Count; i++)
        {
            if (dialogManager.GetCurrentIndexDialogPoint() == tutorialBlocks[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == tutorialBlocks[i].indexDialog)
            {
                if (tutorialBlocks[i].needTargetFullEndDialog == false)
                {
                    WorkbenchBlock.OffOnCollider(tutorialBlocks[i].WorkbenchBlock);
                    AquariumBlock.OffOnCollider(tutorialBlocks[i].AquariumBlock);
                    FridgeBlock.OffOnCollider(tutorialBlocks[i].FridgeBlock);
                    PrinterBlock.OffOnCollider(tutorialBlocks[i].PrinterBlock);
                    TVBlock.OffOnCollider(tutorialBlocks[i].TVBlock);
                    ModelBoardBlock.OffOnCollider(tutorialBlocks[i].ModelBoardBlock);
                    break;
                }
                //Тут4
            }
        }
    }
    public void SetBlockFullEndDialog(Dialog dialog)
    {
        for (int i = 0; i < tutorialBlocks.Count; i++)
        {
            if (dialogManager.GetCurrentIndexDialogPoint() == tutorialBlocks[i].indexDialogPoint && dialogManager.GetCurrentIndexDialog() == tutorialBlocks[i].indexDialog)
            {
                if (tutorialBlocks[i].needTargetFullEndDialog)
                {
                    WorkbenchBlock.OffOnCollider(tutorialBlocks[i].WorkbenchBlock);
                    AquariumBlock.OffOnCollider(tutorialBlocks[i].AquariumBlock);
                    FridgeBlock.OffOnCollider(tutorialBlocks[i].FridgeBlock);
                    PrinterBlock.OffOnCollider(tutorialBlocks[i].PrinterBlock);
                    TVBlock.OffOnCollider(tutorialBlocks[i].TVBlock);
                    ModelBoardBlock.OffOnCollider(tutorialBlocks[i].ModelBoardBlock);
                    break;
                }
                //Тут5
            }
        }
    }

    [Serializable]
    public class BoolBlockObjects
    {
        public int indexDialogPoint = 0;
        public int indexDialog = 0;
        [Space]
        public bool needTargetFullEndDialog = false;
        [Space]
        [Space]
        public bool WorkbenchBlock = false;
        public bool AquariumBlock = false;
        public bool FridgeBlock = false;
        public bool PrinterBlock = false;
        public bool TVBlock = false;
        public bool ModelBoardBlock = false;
        //Тут6
    }
}
