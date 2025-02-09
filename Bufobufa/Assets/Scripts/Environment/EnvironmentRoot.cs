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
        [Header("Table")]
        [SerializeField] private TableWithItems tableWithItems;
        [Header("Board")]
        [SerializeField] private ModelBoard modelBoard;
        [Header("MixTable")]
        [SerializeField] private MixTable mixTable;
        [Header("Aquarium")]
        [SerializeField] private AquariumOpen aquariumOpen;
        [Header("Fridge")]
        [SerializeField] private Fridge.Fridge fridgeOpen;
        [Header("Table")]
        [SerializeField] private TableOpen TableOpen;
        [Header("Shelf")]
        [SerializeField] private TableWithItems shelfWithItems;

        private SaveManager saveManager;

        public void Init(Player player, PlayerMouseMove playerMouseMove, SaveManager saveManager)
        {
            this.saveManager = saveManager;

            for (int i = 0; i < nextRooms.Count; i++)
            {
                nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
            }

            tableWithItems.Init(saveManager, player);
            mixTable.Init(saveManager);
            modelBoard.Init(saveManager, mixTable, player, playerMouseMove);
            aquariumOpen.Init(saveManager, player, playerMouseMove);
            fridgeOpen.Init(saveManager, player, playerMouseMove);
            TableOpen.Init(saveManager, player, playerMouseMove);
            shelfWithItems.Init(saveManager, player);
        }
        public void OnUpdate(float deltaTime)
        {
            modelBoard.OnUpdate(deltaTime);
            aquariumOpen.OnUpdate(deltaTime);
            fridgeOpen.OnUpdate(deltaTime);
            TableOpen.OnUpdate(deltaTime);
        }
    }
}
