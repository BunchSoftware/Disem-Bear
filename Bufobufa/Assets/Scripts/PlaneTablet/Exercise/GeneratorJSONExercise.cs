using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorJSONExercise : MonoBehaviour
{
    [SerializeField] private List<Exercise> exercises  = new List<Exercise>();
    [TextAreaAttribute(10, 100)]
    [SerializeField] private string jsonOutput;

    private void OnValidate()
    {
        jsonOutput = JsonConvert.SerializeObject(exercises);
    }
}
