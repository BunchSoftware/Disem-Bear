using System;
using UI.PlaneTablet.Exercise;

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
