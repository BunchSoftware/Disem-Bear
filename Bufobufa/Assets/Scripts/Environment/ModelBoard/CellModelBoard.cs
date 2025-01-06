using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Environment.ModelBoard
{
    public class CellModelBoard : MonoBehaviour
    {
        [SerializeField] private int indexCell = 0;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private ModelBoard modelBoard;

        public void Init(ModelBoard modelBoard)
        {
            this.modelBoard = modelBoard;
            this.modelBoard.OnPickUpItem.AddListener((item) =>
            {
                OnPickUpItem?.Invoke(item);
            });

            this.modelBoard.OnPutItem.AddListener((item) =>
            {
                OnPutItem?.Invoke(item);
            });
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
