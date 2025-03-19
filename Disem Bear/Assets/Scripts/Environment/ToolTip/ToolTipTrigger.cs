using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager._instance.ToolTipOn(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager._instance.ToolTipOff();
    }
}
