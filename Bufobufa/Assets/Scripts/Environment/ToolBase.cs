using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBase : MonoBehaviour
{
    [SerializeField] private string toolTipText;

    private void OnMouseEnter()
    {
        ToolTipManager._instance.ToolTipOn(toolTipText);
    }
    private void OnMouseExit()
    {
        ToolTipManager._instance.ToolTipOff();
    }
}
