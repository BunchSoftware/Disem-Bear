using External.DI;
using Game.Environment.Aquarium;
using Game.Environment.LMixTable;
using Game.Environment.LModelBoard;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using Game.Music;
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
        public TV tv;
        [Header("Table")]
        [SerializeField] private Workbench workbench;
        [Header("Table")]
        [SerializeField] private List<TableWithItems> tablesWithItems;
        [Header("Board")]
        [SerializeField] private List<ModelBoard> modelBoards;
        [Header("Aquarium")]
        [SerializeField] private List<Aquarium.Aquarium> aquariums;
        [Header("Fridge")]
        [SerializeField] private List<Fridge.Fridge> fridges;
        [Header("Printer")]
        [SerializeField] private Printer.Printer printer;
        [Header("PostTube")]
        [SerializeField] private PostTube postTube;
        [Header("Trash")]
        [SerializeField] private Trash trash;

        public void Init(Player player, PlayerMouseMove playerMouseMove, SoundManager soundManager, ExerciseManager exerciseManager, ToastManager toastManager, GameBootstrap gameBootstrap)
        {
            workbench.Init(player, playerMouseMove, gameBootstrap);
            for (int i = 0; i < nextRooms.Count; i++)
            {
                nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
            }

            for (int i = 0; i < tablesWithItems.Count; i++)
            {
                tablesWithItems[i].Init(player, gameBootstrap);
            }

            for (int i = 0; i < aquariums.Count; i++)
            {
               aquariums[i].Init(player, playerMouseMove, gameBootstrap);
            }
            for (int i = 0; i < fridges.Count; i++)
            {
                fridges[i].Init(player, playerMouseMove);
            }
            for (int i = 0; i < modelBoards.Count; i++)
            {
                modelBoards[i].Init(workbench, player, playerMouseMove, gameBootstrap);
            }
            tv.Init(playerMouseMove, player, gameBootstrap);
            printer.Init(soundManager, player, gameBootstrap);
            postTube.Init(player, exerciseManager, toastManager, gameBootstrap);
            trash.Init(player, gameBootstrap, toastManager);

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
            trash.OnUpdate(deltaTime);
        }
    }
}
