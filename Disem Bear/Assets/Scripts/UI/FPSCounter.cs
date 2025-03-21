using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FPSColor
{
    public Color color;
    public int minFPS;
}

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private List<FPSColor> fpsCounterList;
    private Text textCounter;
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    private void Start()
    {
        textCounter = GetComponent<Text>();
        frameDeltaTimeArray = new float[60];
    }

    private void Update()
    {
        frameDeltaTimeArray[lastFrameIndex++] = Time.unscaledDeltaTime;

        if (lastFrameIndex >= frameDeltaTimeArray.Length)
            lastFrameIndex = 0;

        textCounter.text = "FPS: " + Mathf.RoundToInt(CalculateFPS()).ToString();

        for (int i = 0; i < fpsCounterList.Count; i++)
        {
            if (Mathf.RoundToInt(CalculateFPS()) >= fpsCounterList[i].minFPS)
            {
                textCounter.color = fpsCounterList[i].color;
                break;
            }
        }
    }

    private float CalculateFPS()
    {
        float total = 0;
        for (int i = 0; i < frameDeltaTimeArray.Length; i++)
        {
            total += frameDeltaTimeArray[i];
        }
        return frameDeltaTimeArray.Length / total;
    }
}
