using Game.LPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Game.Environment.Item
{
    public enum TypePickUpItem
    {
        None = 0,
        PickUpItem = 1,
        Package = 2
    }
    public class PickUpItem : MonoBehaviour, IRightMouseDownClickable, ILeftMouseDownClickable
    {
        public bool IsClickedLeftMouseButton = false;
        public bool IsClickedRightMouseButton = false;

        public bool CanTakeByCollisionPlayer = true;
        public TypePickUpItem TypeItem => typeItem;
        [SerializeField] private TypePickUpItem typeItem = TypePickUpItem.None;

        public string NameItem => nameItem;
        [SerializeField] private string nameItem;

        public void OnMouseLeftClickDownObject()
        {
            IsClickedLeftMouseButton = true;
        }

        public void OnMouseLeftClickDownOtherObject()
        {
            IsClickedLeftMouseButton = false;
        }

        public void OnMouseRightClickDownObject()
        {
            IsClickedRightMouseButton = true;
        }

        public void OnMouseRightClickDownOtherObject()
        {
            IsClickedRightMouseButton = false;
        }
    }
}
