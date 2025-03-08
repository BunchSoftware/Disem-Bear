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
        [SerializeField] private PostTube postTube;
        [SerializeField] private ModelBoard modelBoard;
        [SerializeField] private MakePathToObject makePathToObject;

        private DialogManager dialogManager;
        private Player player;

        public void Init(DialogManager dialogManager, Player player, SaveManager saveManager)
        {
            this.dialogManager = dialogManager;
            this.player = player;

            makePathToObject.Init(player);

            pointerTutorialManager.Init(dialogManager, player, makePathToObject);

            Debug.Log("TutorialRoot: Успешно иницилизирован");
        }

        public void OnUpdate(float deltaTime)
        {
            makePathToObject.OnUpdate(deltaTime);
        }
    }
}
