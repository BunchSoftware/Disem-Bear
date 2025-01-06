using DG.Tweening.Core.Easing;
using External.DI;
using External.Storage;
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

        private SaveManager saveManager;

        public void Init(PlayerMouseMove playerMouseMove, SaveManager saveManager)
        {
            this.saveManager = saveManager;

            for (int i = 0; i < nextRooms.Count; i++)
            {
                nextRooms[i].Init(playerMouseMove, invisibleWallBetweenRooms);
            }

            tableWithItems.Init(saveManager);
            modelBoard.Init(saveManager);
        }
        public void OnUpdate(float deltaTime)
        {

        }
    }
}
