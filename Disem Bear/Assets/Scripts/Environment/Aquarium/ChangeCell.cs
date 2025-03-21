using UnityEngine;


namespace Game.Environment.Aquarium
{
    public class ChangeCell : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Aquarium aquarium;
        [SerializeField] private bool ChangeToLeft = true;

        public Color OriginalColor;
        public Color OnEnterButton;
        public Color OnCLickButton;

        public void Init(Aquarium aquarium)
        {
            this.aquarium = aquarium;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = OriginalColor;
        }

        private void OnMouseEnter()
        {
            spriteRenderer.color = OnEnterButton;
        }
        private void OnMouseExit()
        {
            spriteRenderer.color = OriginalColor;
        }
        private void OnMouseDown()
        {
            spriteRenderer.color = OnCLickButton;
            if (ChangeToLeft)
            {
                aquarium.ChangeCellLeft();
            }
            else
            {
                aquarium.ChangeCellRight();
            }
        }
        private void OnMouseUp()
        {
            spriteRenderer.color = OriginalColor;
        }
        public void SetOff()
        {
            gameObject.SetActive(false);
        }
        public void SetOn()
        {
            gameObject.SetActive(true);
        }
    }
}
