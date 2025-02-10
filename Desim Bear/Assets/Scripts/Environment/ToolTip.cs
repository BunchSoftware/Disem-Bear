using Game.Environment.LMixTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private IngradientSpawner ingradientSpawner; // Ссылка на скрипт IngradientSpawner
    [SerializeField] private string toolTipText; // Текст подсказки

    private void OnMouseEnter()
    {
        // Получаем количество ингредиентов из IngradientSpawner
        int count = ingradientSpawner.GetIngradient().countIngradient;

        // Формируем текст подсказки с количеством ингредиентов
        string fullToolTipText = $"{toolTipText}: {count}";

        // Показываем подсказку
        ToolTipManager._instance.ToolTipOn(fullToolTipText);
    }

    private void OnMouseExit()
    {
        // Скрываем подсказку
        ToolTipManager._instance.ToolTipOff();
    }
}
