using System.Collections.Generic;
using UnityEngine;

public enum DialogType { Text, Choice }

public class DialogueTrigger : MonoBehaviour
{
    public List<Dialogue> dialogue = new List<Dialogue>();

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.DialogEnded.AddListener(() => Destroy(gameObject));
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }
}


[System.Serializable]
public class DialogueText
{
    public string name;
    public Sprite icon;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class DialogueChoice
{
    public string name;
    public Sprite icon;
    public string choice1;
    public bool ReturnToChoice1 = true;
    public GameObject choiceObj1;
    [Space] [Space]
    public string choice2;
    public bool ReturnToChoice2 = true;
    public GameObject choiceObj2;
    [Space] [Space]
    public string choice3;
    public bool ReturnToChoice3 = true;
    public GameObject choiceObj3;
}

[System.Serializable]
public class Dialogue
{
    public DialogType dialogType;

    public DialogueText DialogueText;

    public DialogueChoice DialogueChoice;

}
