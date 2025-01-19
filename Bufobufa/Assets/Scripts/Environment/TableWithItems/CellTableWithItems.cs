using Game.Environment.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Environment.LTableWithItems
{
    public class CellTableWithItems : MonoBehaviour
    {
        [SerializeField] private int indexCell = 0;

        public UnityEvent<PickUpItem> OnPickUpItem;
        public UnityEvent<PickUpItem> OnPutItem;

        private TableWithItems tableWithItems;

        public void Init(TableWithItems tableWithItems)
        {
            this.tableWithItems = tableWithItems;
            this.tableWithItems.OnPickUpItem.AddListener((item) =>
            {
                OnPickUpItem?.Invoke(item);
            });

            this.tableWithItems.OnPutItem.AddListener((item) =>
            {
                OnPutItem?.Invoke(item);
            });
        }

        public PickUpItem PickUpItem()
        {
            return tableWithItems.PickUpItem(indexCell);
        }

        public bool PutItem(PickUpItem pickUpItem)
        {
            return tableWithItems.PutItem(pickUpItem, indexCell);
        }
    }
}
