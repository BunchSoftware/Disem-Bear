using Game.Environment;
using Game.Environment.LMixTable;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class ToolTip : MonoBehaviour, IMouseOver
{
    [SerializeField] private IngradientSpawner ingradientSpawner; // Ссылка на скрипт IngradientSpawner
    [SerializeField] private string toolTipText; // Текст подсказки

    public void OnMouseEnterObject()
    {
        // Получаем количество ингредиентов из IngradientSpawner
        int count = ingradientSpawner.GetIngradient().countIngradient;

        // Формируем текст подсказки с количеством ингредиентов
        string fullToolTipText = $"{toolTipText}: {count}";

        // Показываем подсказку
        ToolTipManager._instance.ToolTipOn(fullToolTipText);
    }

    public void OnMouseExitObject()
    {
        // Скрываем подсказку
        ToolTipManager._instance.ToolTipOff();
    }
}
