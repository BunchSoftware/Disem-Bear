using Game.Environment;
using Game.Environment.LMixTable;
using UnityEngine;

public class ToolTip : MonoBehaviour, IMouseOver
{
    [SerializeField] private IngradientSpawner ingradientSpawner; // ������ �� ������ IngradientSpawner
    [SerializeField] private string toolTipText; // ����� ���������

    public void OnMouseEnterObject()
    {
        // �������� ���������� ������������ �� IngradientSpawner
        int count = ingradientSpawner.GetIngradient().countIngradient;

        // ��������� ����� ��������� � ����������� ������������
        string fullToolTipText = $"{toolTipText}: {count}";

        // ���������� ���������
        ToolTipManager._instance.ToolTipOn(fullToolTipText);
    }

    public void OnMouseExitObject()
    {
        // �������� ���������
        ToolTipManager._instance.ToolTipOff();
    }
}
