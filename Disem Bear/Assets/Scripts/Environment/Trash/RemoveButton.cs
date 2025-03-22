using System.Collections;
using System.Collections.Generic;
using Game.Environment;
using UnityEngine;

public class RemoveButton : MonoBehaviour, ILeftMouseDownClickable
{
    private bool isClick = false;
    private TriggerObject triggerObject;


    public void Init(TriggerObject triggerObject)
    {
        this.triggerObject = triggerObject;
        triggerObject.OnTriggerStayEvent.AddListener((collider) =>
        {

        });
    }

    public void OnMouseLeftClickDownObject()
    {
        isClick = true;
    }

    public void OnMouseLeftClickDownOtherObject()
    {
        isClick = false;
    }
}
