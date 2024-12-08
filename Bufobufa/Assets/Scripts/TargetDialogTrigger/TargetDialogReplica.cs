using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDialogReplica : MonoBehaviour
{
    public string TextReplica = "";
    public int NumPointer = -1;

    private DialogManager DialogManager;
    private AllPointerManager AllPointerManager;

    private void Start()
    {
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        AllPointerManager = GameObject.Find("AllPointerManager").GetComponent<AllPointerManager>();
        DialogManager.OnStartDialog.AddListener(TargetReplica);
    }

    public void TargetReplica(Dialog dialog)
    {
        if (dialog.textDialog == TextReplica && NumPointer != -1)
        {
            AllPointerManager.SetPointer(NumPointer);
        }
    }
}
