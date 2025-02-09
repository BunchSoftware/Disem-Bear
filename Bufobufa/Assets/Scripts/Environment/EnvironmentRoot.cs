using DG.Tweening.Core.Easing;
using External.DI;
using External.Storage;
using Game.Environment.Aquarium;
using Game.Environment.Fridge;
using Game.Environment.LMixTable;
using Game.Environment.LModelBoard;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment
{
    public class EnvironmentRoot : MonoBehaviour, IUpdateListener
    {
        [Header("Transition Between Rooms")]
        [SerializeField] private List<NextRoom> nextRooms;
        [SerializeField] private GameObject invisibleWallBetweenRooms;
        [Header("MixTable")]
        [SerializeField] private MixTable mixTable;
        [Header("Table")]
        [SerializeField] private TableOpen tableOpen;
        [Header("Table")]
        [SerializeField] private List<TableWithItems> tablesWithItems;
        [Header("Board")]
        [SerializeField] private List<ModelBoard> modelBoards;
        [Header("Aquarium")]
        [SerializeField] private List<AquariumOpen> aquariumOpens;
        [Header("Fridge")]
        [SerializeField] private List<Fridge.Fridge> fridges;

        private SaveManager saveManager;

        public void Init(Player player, PlayerMouseMove playerMouseMove, SaveManager saveManager)
        {
            this.saveManager = saveManager;

            for (int i = 0; i < nextRooms.Count; i++)
            {
                nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
            }

            for (int i = 0; i < tablesWithItems.Count; i++)
            {
                tablesWithItems[i].Init(saveManager, player);
            }

            mixTable.Init(saveManager);

            for (int i = 0; i < modelBoards.Count; i++)
            {
                modelBoards[i].Init(saveManager, mixTable, player, playerMouseMove);
            }
            for (int i = 0; i < aquariumOpens.Count; i++)
            {
                aquariumOpens[i].Init(saveManager, player, playerMouseMove);
            }
            for (int i = 0; i < fridges.Count; i++)
            {
                fridges[i].Init(saveManager, player, playerMouseMove);
            }
            tableOpen.Init(saveManager, player, playerMouseMove);
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

            tableOpen.OnUpdate(deltaTime);
        }
    }
}
