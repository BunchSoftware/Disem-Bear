using System;
using System.Collections;
using System.Collections.Generic;
using UI.PlaneTablet.Exercise;
using UnityEngine;

namespace External.Storage
{
    [Serializable]
    public class ExerciseData
    {
        public string nameExercise;
        public TypeOfExerciseCompletion typeOfExerciseCompletion;
        public bool isVisible = true;
        public bool isGetExerciesItems;
    }
}
