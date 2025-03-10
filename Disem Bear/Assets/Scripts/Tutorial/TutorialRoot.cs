using External.Storage;
using Game.LDialog;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Environment.LModelBoard;
using External.DI;

namespace Game.Tutorial
{
    public class TutorialRoot : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private PointerTutorialManager pointerTutorialManager;
        [SerializeField] private BlockTutorialManager blockTutorialManager;
        [SerializeField] private PostTube postTube;
        [SerializeField] private ModelBoard modelBoard;

        private DialogManager dialogManager;
        private Player player;

        public void Init(DialogManager dialogManager, Player player, SaveManager saveManager)
        {
            this.dialogManager = dialogManager;
            this.player = player;

            pointerTutorialManager.Init(dialogManager, player);

            blockTutorialManager.Init(dialogManager);

            Debug.Log("TutorialRoot: ������� ��������������");
        }

        public void OnUpdate(float deltaTime)
        {
            pointerTutorialManager.OnUpdate(deltaTime);
            blockTutorialManager.OnUpdate(deltaTime);
        }
    }
}
