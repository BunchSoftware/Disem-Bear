using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorJSONDialog : MonoBehaviour
{
   [SerializeField] private List<DialogPoint> dialogPoints = new List<DialogPoint>();
   [SerializeField] private TextAsset textOutput;
   [TextAreaAttribute(10, 100)]
   [SerializeField] private string jsonOutput;

   private void OnValidate()
   {
        for (int i = 0; i < dialogPoints.Count; i++)
        {
            for (int j = 0; j < dialogPoints[i].dialog.Count; j++)
            {
                dialogPoints[i].dialog[j].jsonHTMLColorRGBA = "#" + ColorUtility.ToHtmlStringRGBA(dialogPoints[i].dialog[j].colorText);
                dialogPoints[i].dialog[j].pathToFont = AssetDatabase.GetAssetPath(dialogPoints[i].dialog[j].fontText);

                if (dialogPoints[i].dialog[j].avatar != null)
                    dialogPoints[i].dialog[j].pathToAvatar = AssetDatabase.GetAssetPath(dialogPoints[i].dialog[j].avatar) + "#" + dialogPoints[i].dialog[j].avatar.name;
            }
        }

        jsonOutput = JsonConvert.SerializeObject(dialogPoints);

        if(textOutput != null)
            File.WriteAllText(AssetDatabase.GetAssetPath(textOutput), jsonOutput, Encoding.UTF8);
    }
}
