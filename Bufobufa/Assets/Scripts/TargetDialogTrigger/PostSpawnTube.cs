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
        AllPointerManager = GameObject.Find("AllPointerManager").GetComponent<AllPointerManager>();
        DialogManager.EndDialog.AddListener(DropBox);
    }
    public void DropBox(Dialog dialog)
    {
        if (dialog.textDialog == "ќ, а вот и посылка. Ёта почтова€ труба действительно быстра€. Ѕери, бери, это дл€ теб€.")
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
