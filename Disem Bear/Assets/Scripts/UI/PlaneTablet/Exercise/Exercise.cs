using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Exercise
{
    [Serializable]
    public class Exercise
    {
        private bool isCompletedExercise = false;

        public List<Reward> exerciseRewards;
        public string header;
        [TextArea(10, 100)]
        public string description;
        public Sprite avatar;

        public string conditionExercise;

        public bool GetIsCompletedExercise()
        {
            return isCompletedExercise;
        }

        public List<Reward> DoneExercise(string messageCondition)
        {
            if (conditionExercise == messageCondition)
            {
                isCompletedExercise = true;
                return exerciseRewards;
            }
            else
                return null;
        }
    }
}
