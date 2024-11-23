using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorJSONExercise : MonoBehaviour
{
    [SerializeField] private List<Exercise> exercises  = new List<Exercise>();
    [SerializeField] private TextAsset textOutput;
    [TextAreaAttribute(10, 100)]
    [SerializeField] private string jsonOutput;

    private void OnValidate()
    {
        for (int i = 0; i < exercises.Count; i++)
        { 
            exercises[i].pathToAvatar = AssetDatabase.GetAssetPath(exercises[i].avatar);
            if (exercises[i].avatar != null)
                exercises[i].pathToAvatar = AssetDatabase.GetAssetPath(exercises[i].avatar) + "#" + exercises[i].avatar.name;
        }

        jsonOutput = JsonConvert.SerializeObject(exercises);

        if (textOutput != null)
            File.WriteAllText(AssetDatabase.GetAssetPath(textOutput), jsonOutput, Encoding.UTF8);
    }


}
