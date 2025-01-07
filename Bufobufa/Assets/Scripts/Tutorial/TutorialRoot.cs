using External.Storage;
using Game.LDialog;
using Game.Environment.LPostTube;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialRoot : MonoBehaviour
    {
        [SerializeField] private PointerTutorialManager pointerTutorialManager;
        [SerializeField] private PostTube postTube;

        private DialogManager dialogManager;
        private Player player;

        public void Init(DialogManager dialogManager, Player player, SaveManager saveManager)
        {
            this.dialogManager = dialogManager;
            this.player = player;

            pointerTutorialManager.Init(dialogManager, player);
        }
    }
}
