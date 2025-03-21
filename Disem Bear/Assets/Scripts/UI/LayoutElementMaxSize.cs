using TMPro;
using UnityEngine;

public class LayoutElementMaxSize : MonoBehaviour
{
    private RectTransform rect;
    private RectTransform parentRect;
    [SerializeField] private float maxCountSymbolInLine = 20;
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        rect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            Mathf.Min(textMeshPro.preferredWidth, textMeshPro.preferredWidth / textMeshPro.text.Length * maxCountSymbolInLine)
        );
    }
}
