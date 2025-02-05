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
    public class PickUpItem : MonoBehaviour, ILeftMouseClickable, IRightMouseClickable
    {
        public bool IsClickedLeftMouseButton = false;
        public bool IsClickedRightMouseButton = false;

        public bool CanTakeByCollisionPlayer = true;
        public TypePickUpItem TypeItem => typeItem;
        [SerializeField] private TypePickUpItem typeItem = TypePickUpItem.None;

        public string NameItem => nameItem;
        [SerializeField] private string nameItem;

        public void OnMouseLeftClickObject()
        {
            IsClickedLeftMouseButton = true;
        }

        public void OnMouseLeftClickOtherObject()
        {
            IsClickedLeftMouseButton = false;
        }

        public void OnMouseRightClickObject()
        {
            IsClickedRightMouseButton = true;
        }

        public void OnMouseRightClickOtherObject()
        {
            IsClickedRightMouseButton = false;
        }
    }
}
