using External.API;
using External.Storage;
using Game.Environment;
using Game.Environment.Item;
using Game.LPlayer;
using Game.Tutorial;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.AI;

namespace External.DI
{
    public class GameBootstrap : Bootstrap
    {
        [SerializeField] private Fade fade;
        [Header("Player")]
        [SerializeField] private Player player;
        [SerializeField] private PlayerChangeImage playerChangeImage = new();
        [SerializeField] private PlayerMouseMove playerMouseMove;
        [Header("Tutorial")]
        [SerializeField] private TutorialRoot tutorialRoot;
        [Header("Environment")]
        [SerializeField] private EnvironmentRoot environmentRoot;
        public AudioClip ScaleChooseObjectSound;
        [Header("UI")]
        [SerializeField] private UIGameRoot uiGameRoot;
        [SerializeField] private ToastManager toastManager;
        [SerializeField] private PrefabsPickUpItemsDatabase filePrefabsPickUpItems;
        private static PrefabsPickUpItemsDatabase s_filePrefabsPickUpItems;
        [SerializeField] private EventsManager eventsManager;

        [SerializeField] private PlayerInput playerInput = new();

        private const float WaitTimePanel = 0.7f;

        private void Awake()
        {
            fade.FadeWhite();
            StartCoroutine(IWaitFadePanel(WaitTimePanel));


            GlobalEvent globalEvent = new GlobalEvent();
            globalEvent.name = "1252";
            globalEvent.text = "25265151414";
            globalEvent.start_date_time = "152";
            globalEvent.once_in_hours = 1;
            globalEvent.duration_in_minutes = 52;
            //SaveManager.CreateGlobalEvent(globalEvent);
        }

        private IEnumerator IWaitFadePanel(float time)
        {
            yield return new WaitForSeconds(time);

            Time.timeScale = TimeScale;

            s_filePrefabsPickUpItems = filePrefabsPickUpItems;
            #region Init
            if (!player)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� Player");
                yield return null;
            }

            if (!tutorialRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� TutorialRoot");
                yield return null;
            }

            if (!environmentRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� EnvironmentRoot");
                yield return null;
            }

            if (!uiGameRoot)
            {
                Debug.LogError("CriticError-Bootstrap: �� ������� �������� ���������� UIGameRoot");
                yield return null;
            }
            #endregion
            Init();
            toastManager.Init(soundManager);

            eventsManager.Init(toastManager);
            updateListeners.Add(eventsManager);


            player.Init();
            playerChangeImage.Init(player.gameObject.GetComponent<Animator>(), player);
            playerMouseMove.OnMove += playerChangeImage.Update;

            updateListeners.Add(playerMouseMove);
            playerMouseMove.Init(this, player.gameObject.GetComponent<NavMeshAgent>(), this);

            updateListeners.Add(environmentRoot);
            environmentRoot.Init(player, playerMouseMove, soundManager, uiGameRoot.GetExerciseManager(), toastManager, this);

            updateListeners.Add(tutorialRoot);
            tutorialRoot.Init(uiGameRoot.GetDialogManager(), player);

            updateListeners.Add(uiGameRoot);
            uiGameRoot.Init(environmentRoot.tv, soundManager, toastManager);

            updateListeners.Add(playerInput);

            Debug.Log("GameBootstrap: ������� �������������� ��� ����� ����");
        }


        public static PickUpItem FindPickUpItemToPrefabs(string nameItem)
        {
            for (int i = 0; i < s_filePrefabsPickUpItems.pickUpItems.Count; i++)
            {
                if (s_filePrefabsPickUpItems.pickUpItems[i].NameItem == nameItem && nameItem.Length >= 1)
                    return s_filePrefabsPickUpItems.pickUpItems[i];
            }

            return null;
        }
    }
}
