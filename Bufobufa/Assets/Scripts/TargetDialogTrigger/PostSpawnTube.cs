using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PostSpawnTube : MonoBehaviour
{
    private DialogManager DialogManager;
    private PostOfficeTube PostOfficeTube;
    private AllPointerManager AllPointerManager;

    private bool OneTap = true;

    private void Start()
    {
        DialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        PostOfficeTube = GameObject.Find("PostOfficeTube").GetComponent<PostOfficeTube>();
        AllPointerManager = GameObject.Find("AllPointerManager").GetComponent<AllPointerManager>();
        DialogManager.SendInputFieldText.AddListener(DropBox);
    }
    public void DropBox(string s)
    {
        if (OneTap)
        {
            OneTap = false;
            PostOfficeTube.ItemExist = true;
            PostOfficeTube.ObjectFall();
            StartCoroutine(WaitBoxFall(1f));
        }
    }
    IEnumerator WaitBoxFall(float f)
    {
        yield return new WaitForSeconds(f);
        AllPointerManager.SetPointer(0);
    }
}
