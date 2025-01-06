using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Exercise
{
    [CreateAssetMenu(fileName = "New Exercise", menuName = "Exercise")]
    public class FileExercise : ScriptableObject
    {
        public List<Exercise> exercises;
    }
}
