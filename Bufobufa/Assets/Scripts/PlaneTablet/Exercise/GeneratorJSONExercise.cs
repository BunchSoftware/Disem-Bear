using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratorJSONExercise : MonoBehaviour
{
    [SerializeField] private List<Exercise> exercises  = new List<Exercise>();
    [TextAreaAttribute(10, 100)]
    [SerializeField] private string jsonOutput;

    private void OnValidate()
    {
        for (int i = 0; i < exercises.Count; i++)
        { 
            exercises[i].pathToAvatar = AssetDatabase.GetAssetPath(exercises[i].avatar);
        }

        jsonOutput = JsonConvert.SerializeObject(exercises);
    }
}
