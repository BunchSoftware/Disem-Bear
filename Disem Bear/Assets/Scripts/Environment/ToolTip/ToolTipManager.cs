using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.LDialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;
    [SerializeField] private TextMeshProUGUI ToolTipText;
    [Tooltip("Time Fade ToolTip")]
    [SerializeField] private float timeOfAppearanceToolTip = 0.5f;
    [SerializeField] private float timeOfDissaperenceToolTip = 0.5f;

    private Image ToolTipImage;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        ToolTipImage = GetComponent<Image>();
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ToolTipOn(string message)
    {
        ToolTipOn(message, timeOfAppearanceToolTip);
    }

    public void ToolTipOn(string message, float timeOfAppearanceToolTip)
    {
        StopAllCoroutines();
        ToolTipText.text = message;
        ToolTipImage.DOFade(1f, timeOfAppearanceToolTip);
        ToolTipText.DOFade(1f, timeOfAppearanceToolTip);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }


    public void ToolTipOff()
    {
        ToolTipOff(timeOfDissaperenceToolTip);
    }

    public void ToolTipOff(float timeOfDissaperenceToolTip)
    {
        ToolTipImage.DOFade(0f, timeOfDissaperenceToolTip);
        ToolTipText.DOFade(0f, timeOfDissaperenceToolTip);
        StartCoroutine(ITextEmpty(timeOfDissaperenceToolTip));
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    private IEnumerator ITextEmpty(float time)
    {
        yield return new WaitForSeconds(time);
        ToolTipText.text = string.Empty;
    }
}
