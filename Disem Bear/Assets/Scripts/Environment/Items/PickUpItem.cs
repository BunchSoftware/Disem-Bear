using UnityEngine;

namespace Game.Environment.Item
{
    public enum TypePickUpItem
    {
        None = 0,
        ModelBoardItem = 1,
        PickUpItem = 2,
        Package = 3,
        AquariumMaterial = 4
    }
    [RequireComponent(typeof(ToolBase))]
    public class PickUpItem : MonoBehaviour, IRightMouseDownClickable, ILeftMouseDownClickable
    {
        public bool IsClickedLeftMouseButton = false;
        public bool IsClickedRightMouseButton = false;

        public bool CanTakeByCollisionPlayer = true;
        public TypePickUpItem TypeItem => typeItem;
        [SerializeField] private TypePickUpItem typeItem = TypePickUpItem.None;

        public string NameItem;

        private Quaternion rotation;
        private ToolBase toolBase;

        private void Awake()
        {
            rotation = transform.rotation;
            toolBase = GetComponent<ToolBase>();
            toolBase.toolTipText = NameItem;
        }

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

        public void ResetRotation()
        {
            transform.rotation = rotation;
        }
    }
}
