using External.Storage;
using Game.Environment.Item;
using Game.Environment.LMixTable;
using Game.Environment.LPostTube;
using Game.Environment.LTableWithItems;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Game.Environment.LModelBoard
{
    public class CellModelBoard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private int indexCell = 0;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private ModelBoard modelBoard;
        private Player player;

        public void Init(ModelBoard modelBoard, Player player)
        {
            this.modelBoard = modelBoard;
            this.player = player;

            this.modelBoard.OnPickUpItem.AddListener((item) =>
            {
                OnPickUpItem?.Invoke(item);
            });

            this.modelBoard.OnPutItem.AddListener((item) =>
            {
                OnPutItem?.Invoke(item);
            });
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                if(player.PlayerPickUpItem)
                    player.PickUpItem(PickUpItem());
                else if(player.PlayerPickUpItem == false)
                {
                    if (PutItem(player.GetPickUpItem()))
                        player.PutItem();
                }
            }
        }

        public PickUpItem PickUpItem()
        {
            return modelBoard.PickUpItem(indexCell);
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            return modelBoard.PutItem(pickUpItem, indexCell);
        }
}
}
