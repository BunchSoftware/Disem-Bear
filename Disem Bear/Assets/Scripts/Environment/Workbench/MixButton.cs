using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Environment.LMixTable
{
    public class MixButton : MonoBehaviour
    {
        private Workbench workbench;
        

        public void Init(Workbench workbench)
        {
            this.workbench = workbench;
        }

        private void OnMouseDown()
        {
            workbench.MixIngradients();
            transform.parent.gameObject.GetComponent<Animator>().Play("ButtonPress");
        }
    }
}
