using External.DI;
using Game.Environment.LModelBoard;
using Game.Environment.LPostTube;
using Game.LDialog;
using Game.LPlayer;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialRoot : MonoBehaviour, IUpdateListener
    {
        [SerializeField] private PointerTutorialManager pointerTutorialManager;
        [SerializeField] private BlockTutorialManager blockTutorialManager;
        [SerializeField] private SendPackageTutorialManager sendPackageTutorialManager;
        [SerializeField] private MainConditionManager mainConditionManager;
        [SerializeField] private PostTube postTube;
        [SerializeField] private ModelBoard modelBoard;

        private DialogManager dialogManager;
        private Player player;

        public void Init(DialogManager dialogManager, Player player)
        {
            this.dialogManager = dialogManager;
            this.player = player;

            pointerTutorialManager.Init(dialogManager, player);

            blockTutorialManager.Init(dialogManager);

            sendPackageTutorialManager.Init(dialogManager);

            mainConditionManager.Init(dialogManager);

            Debug.Log("TutorialRoot: Успешно иницилизирован");
        }

        public void OnUpdate(float deltaTime)
        {
            if (pointerTutorialManager != null)
                pointerTutorialManager.OnUpdate(deltaTime);
            if (blockTutorialManager != null)
                blockTutorialManager.OnUpdate(deltaTime);
        }
    }
}
