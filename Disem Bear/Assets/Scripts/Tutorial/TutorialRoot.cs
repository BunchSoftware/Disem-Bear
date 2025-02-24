using External.Storage;
using Game.LDialog;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Environment.LModelBoard;

namespace Game.Tutorial
{
    public class TutorialRoot : MonoBehaviour
    {
        [SerializeField] private PointerTutorialManager pointerTutorialManager;
        [SerializeField] private PostTube postTube;
        [SerializeField] private ModelBoard modelBoard;

        private DialogManager dialogManager;
        private Player player;

        public void Init(DialogManager dialogManager, Player player, SaveManager saveManager)
        {
            this.dialogManager = dialogManager;
            this.player = player;

            pointerTutorialManager.Init(dialogManager, player);

            Debug.Log("TutorialRoot: ������� ��������������");
        }
    }
}
