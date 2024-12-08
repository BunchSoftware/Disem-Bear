using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostSpawnTube : MonoBehaviour
{
    private DialogManager DialogManager;
    private PostOfficeTube PostOfficeTube;
    private AllPointerManager AllPointerManager;

    private void Start()
    {
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        PostOfficeTube = GameObject.Find("PostOfficeTube").GetComponent<PostOfficeTube>();
        AllPointerManager = GameObject.Find("AllPointer").GetComponent<AllPointerManager>();
        DialogManager.EndDialog.AddListener(DropBox);
    }
    public void DropBox(Dialog dialog)
    {
        if (dialog.textDialog == "Я Руми!")
        {
            PostOfficeTube.ObjectFall();
        }
    }
    IEnumerator WaitBoxFall(float f)
    {
        yield return new WaitForSeconds(f);
        AllPointerManager.SetPointer(0);
    }
}
