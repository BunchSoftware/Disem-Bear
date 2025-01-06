using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTutorialManager : MonoBehaviour
{
    private DialogManager dialogManager;
    private Player player;
    private List<PointerTutorial> pointerTutorial = new List<PointerTutorial>();

    public void Init(DialogManager dialogManager, Player player)
    {
        this.dialogManager = dialogManager;
        this.player = player;

        for (int i = 0; i < transform.childCount; i++)
        {
            PointerTutorial pointerTutorial;

            if (transform.GetChild(i).TryGetComponent<PointerTutorial>(out pointerTutorial)) 
            {
                pointerTutorial.Init(dialogManager, this);
                this.pointerTutorial.Add(pointerTutorial);  
            }
        }
    }
    public void SetPointer(int indexPointer)
    {
        for (int i = 0; i < pointerTutorial.Count; i++)
        {
            pointerTutorial[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        pointerTutorial[indexPointer].transform.GetChild(0).gameObject.SetActive(true);
    }
}
