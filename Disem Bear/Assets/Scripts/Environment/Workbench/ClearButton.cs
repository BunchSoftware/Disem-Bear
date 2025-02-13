using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.LMixTable
{
    public class ClearButton : MonoBehaviour
    {
        private Workbench workbench;

        public void Init(Workbench workbench)
        {
            this.workbench = workbench;
        }

        private void OnMouseDown()
        {
            workbench.ClearIngredients();
            transform.parent.gameObject.GetComponent<Animator>().Play("ButtonPress");
        }
    }
}
