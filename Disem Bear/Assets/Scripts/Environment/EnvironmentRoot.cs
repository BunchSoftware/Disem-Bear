using DG.Tweening.Core.Easing;
using External.DI;
using External.Storage;
using Game.Environment.Aquarium;
using Game.Environment.Fridge;
using Game.Environment.LMixTable;
using Game.Environment.LModelBoard;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using Game.Music;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;

namespace Game.Environment
{
    public class EnvironmentRoot : MonoBehaviour, IUpdateListener
    {
        [Header("Transition Between Rooms")]
        [SerializeField] private List<NextRoom> nextRooms;
        [SerializeField] private GameObject invisibleWallBetweenRooms;
        [Header("TV")]
        [SerializeField] private TV tv;
        [Header("Table")]
        [SerializeField] private Workbench workbench;
        [Header("Table")]
        [SerializeField] private List<TableWithItems> tablesWithItems;
        [Header("Board")]
        [SerializeField] private List<ModelBoard> modelBoards;
        [Header("Aquarium")]
        [SerializeField] private List<AquariumOpen> aquariumOpens;
        [Header("Fridge")]
        [SerializeField] private List<Fridge.Fridge> fridges;
        [Header("Printer")]
        [SerializeField] private Printer.Printer printer;
        [Header("PostTube")]
        [SerializeField] private PostTube postTube;

        public void Init(Player player, PlayerMouseMove playerMouseMove, SoundManager soundManager, ExerciseManager exerciseManager, ToastManager toastManager)
        {
            workbench.Init(player, playerMouseMove);
            for (int i = 0; i < nextRooms.Count; i++)
            {
                nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
            }

            for (int i = 0; i < tablesWithItems.Count; i++)
            {
                tablesWithItems[i].Init(player);
            }

            for (int i = 0; i < aquariumOpens.Count; i++)
            {
                aquariumOpens[i].Init(player, playerMouseMove);
            }
            for (int i = 0; i < fridges.Count; i++)
            {
                fridges[i].Init(player, playerMouseMove);
            }
            for (int i = 0; i < modelBoards.Count; i++)
            {
                modelBoards[i].Init(workbench, player, playerMouseMove);
            }
            tv.Init(playerMouseMove, player);
            printer.Init(soundManager, player);
            postTube.Init(player, exerciseManager, toastManager);

            Debug.Log("EnvironmentRoot: Успешно иницилизирован");
        }
        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < modelBoards.Count; i++)
            {
                modelBoards[i].OnUpdate(deltaTime);
            }
            for (int i = 0; i < fridges.Count; i++)
            {
                fridges[i].OnUpdate(deltaTime);
            }
            for (int i = 0; i < aquariumOpens.Count; i++)
            {
                aquariumOpens[i].OnUpdate(deltaTime);
            }

            workbench.OnUpdate(deltaTime);
            tv.OnUpdate(deltaTime);
        }
    }
}
