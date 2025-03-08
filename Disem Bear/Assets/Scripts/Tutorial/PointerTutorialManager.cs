using Game.LDialog;
using Game.LPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class PointerTutorialManager : MonoBehaviour
    {
        private DialogManager dialogManager;
        private Player player;
        private List<PointerTutorial> pointerTutorial = new List<PointerTutorial>();
        private MakePathToObject makePathToObject;

        public void Init(DialogManager dialogManager, Player player, MakePathToObject makePathToObject)
        {
            this.dialogManager = dialogManager;
            this.player = player;
            this.makePathToObject = makePathToObject;

            for (int i = 0; i < transform.childCount; i++)
            {
                PointerTutorial pointerTutorial;

                if (transform.GetChild(i).TryGetComponent<PointerTutorial>(out pointerTutorial))
                {
                    pointerTutorial.Init(player, dialogManager, this);
                    this.pointerTutorial.Add(pointerTutorial);
                }
            }
        }
        public void SetPointer(int indexPointer)
        {
            makePathToObject.SetTarget(pointerTutorial[indexPointer].lineToObjectTransform);
            //for (int i = 0; i < pointerTutorial.Count; i++)
            //{
            //    pointerTutorial[i].transform.GetChild(0).gameObject.SetActive(false);
            //}
            //pointerTutorial[indexPointer].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
