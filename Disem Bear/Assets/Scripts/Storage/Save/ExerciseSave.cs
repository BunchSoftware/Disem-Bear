using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Exercise;
using UnityEngine;

namespace External.Storage
{
    [Serializable]
    public class ExerciseSave
    {
        public int indexExercise;
        public TypeOfExerciseCompletion typeOfExerciseCompletion;
        public bool isGetPackage;
    }
}
