using UnityEngine;
using UnityEngine.UI;

public class VersionController : MonoBehaviour
{
    private Text text;
    private void Awake()
    {
        text = GetComponent<Text>();
        text.text = text.text + " " + Application.version;
    }
}
