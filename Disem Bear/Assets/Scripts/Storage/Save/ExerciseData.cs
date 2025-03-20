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
        public int indexExercise;
        public List<ExerciseData> internalExercisesData;
        public TypeOfExerciseCompletion typeOfExerciseCompletion;
        public bool isVisible = true;
        public bool isGetExerciesItems;
    }
}
