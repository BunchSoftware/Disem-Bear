using Game.Environment;
using UnityEngine;

public class ToolBase : MonoBehaviour, IMouseOver
{
    public string toolTipText;
    public bool on = true;
    private bool mouseOver = false;

    public void OnMouseEnterObject()
    {
        mouseOver = true;
        if (on)
        {
            ToolTipManager._instance.ToolTipOn(toolTipText);
        }
    }

    public void OnMouseExitObject()
    {
        mouseOver = false;
        ToolTipManager._instance.ToolTipOff();
    }

    public void OnToolTip()
    {
        if (on && mouseOver)
        {
            ToolTipManager._instance.ToolTipOn(toolTipText);
        }
    }

    public void OffToolTip()
    {
        if (mouseOver)
            ToolTipManager._instance.ToolTipOff();
    }

}
