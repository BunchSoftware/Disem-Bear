using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public UnityEvent DialogStarted;
    public UnityEvent DialogEnded;

    private TextMeshProUGUI characterName;
    private Image characterIcon; 
    private TextMeshProUGUI dialogueArea;
    private GameObject Choice1Button;
    private GameObject Choice2Button;
    private GameObject Choice3Button;
    private GameObject NextButton;


    private List<Queue<Dialogue>> lines = new List<Queue<Dialogue>>();
    public int NumDialog = 0;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.05f;

    private GameObject _dialog;

    private void Start()
    {
        _dialog = GameObject.Find("DialogPanelActive").transform.GetChild(0).gameObject;
        characterName = _dialog.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        characterIcon = _dialog.transform.Find("Icon").GetComponent<Image>();
        dialogueArea = _dialog.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Choice1Button = _dialog.transform.Find("ButtonChoice1").gameObject;
        Choice2Button = _dialog.transform.Find("ButtonChoice2").gameObject;
        Choice3Button = _dialog.transform.Find("ButtonChoice3").gameObject;
        NextButton = _dialog.transform.Find("ButtonNext").gameObject;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void StartDialogue(List<Dialogue> dialogue)
    {
        if (NumDialog == 0)
            DialogStarted?.Invoke();
        isDialogueActive = true;

        _dialog.SetActive(true);

        lines.Add(new Queue<Dialogue>());
        lines[NumDialog].Clear();

        foreach (Dialogue dialogueLine in dialogue)
        {
            lines[NumDialog].Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines[NumDialog].Count == 0)
        {
            EndDialogue();
            return;
        }

        Dialogue currentLine = lines[NumDialog].Peek();

        if (currentLine.dialogType == DialogType.Text)
        {
            lines[NumDialog].Dequeue();
            Choice1Button.SetActive(false);
            Choice2Button.SetActive(false);
            Choice3Button.SetActive(false);
            dialogueArea.gameObject.SetActive(true);
            NextButton.SetActive(true);
            characterIcon.sprite = currentLine.DialogueText.icon;
            characterName.text = currentLine.DialogueText.name;

            StopAllCoroutines();

            StartCoroutine(TypeSentence(currentLine.DialogueText));
        }
        else
        {
            dialogueArea.gameObject.SetActive(false);
            NextButton.SetActive(false);
            characterIcon.sprite = currentLine.DialogueChoice.icon;
            characterName.text = currentLine.DialogueChoice.name;

            if (currentLine.DialogueChoice.choice1 != "")
            {
                Choice1Button.SetActive(true);
                Choice1Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLine.DialogueChoice.choice1;
            }
            if (currentLine.DialogueChoice.choice2 != "")
            {
                Choice2Button.SetActive(true);
                Choice2Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLine.DialogueChoice.choice2;
            }
            if (currentLine.DialogueChoice.choice3 != "")
            {
                Choice3Button.SetActive(true);
                Choice3Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLine.DialogueChoice.choice3;
            }
        }
    }

    public void ButtonChoice1On()
    {
        Dialogue currentLine = lines[NumDialog].Peek();
        if (currentLine.DialogueChoice.ReturnToChoice1 == false) lines[NumDialog].Dequeue();
        NumDialog += 1;
        currentLine.DialogueChoice.choiceObj1.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    public void ButtonChoice2On()
    {
        Dialogue currentLine = lines[NumDialog].Peek();
        if (currentLine.DialogueChoice.ReturnToChoice2 == false) lines[NumDialog].Dequeue();
        NumDialog += 1;
        currentLine.DialogueChoice.choiceObj2.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    public void ButtonChoice3On()
    {
        Dialogue currentLine = lines[NumDialog].Peek();
        if (currentLine.DialogueChoice.ReturnToChoice3 == false) lines[NumDialog].Dequeue();
        NumDialog += 1;
        currentLine.DialogueChoice.choiceObj3.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    IEnumerator TypeSentence(DialogueText dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void EndDialogue()
    {
        if (NumDialog == 0)
        {
            lines.Clear();
            DialogEnded?.Invoke();
            isDialogueActive = false;
            _dialog.SetActive(false);
        }
        else
        {
            NumDialog -= 1;
            DisplayNextDialogueLine();
        }
    }
}
