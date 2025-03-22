using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Exercise
{
    [CreateAssetMenu(fileName = "ExerciseDatabase", menuName = "Exercise/ExerciseDatabase")]
    public class ExerciseDatabase : ScriptableObject
    {
        public List<Exercise> exercises;
    }
}
