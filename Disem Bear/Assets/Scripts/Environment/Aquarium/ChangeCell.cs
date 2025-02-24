using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.Aquarium
{
    public class ChangeCell : MonoBehaviour
    {
        public int ChangeCellNum = 1;

        private SpriteRenderer sr;

        public Color OriginalColor;
        public Color OnEnterButton;
        public Color OnCLickButton;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.color = OriginalColor;
        }
        private void OnMouseEnter()
        {
            sr.color = OnEnterButton;
        }
        private void OnMouseExit()
        {
            sr.color = OriginalColor;
        }
        private void OnMouseDown()
        {
            sr.color = OnCLickButton;
            //transform.parent.GetComponent<Aquarium>().ChangeCell(ChangeCellNum);
        }
        private void OnMouseUp()
        {
            sr.color = OriginalColor;
        }
    }
}
