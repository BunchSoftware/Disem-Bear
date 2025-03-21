using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Exercise
{
    [Serializable]
    public class Requirement
    {
        public string typeRequirement;
        public int countRequirement = 1;
        public Sprite avatarRequirement;
    }


    [Serializable]
    public class ExerciseItem
    {
        public string typeItem;
        public int countItem = 1;
        public string typeMachineDispensingItem;
        public Sprite avatarItem;
    }

    [Serializable]
    public class Exercise
    {
        private bool isCompletedExercise = false;

        public string nameExercise;
        [Header("Награды, требования, квестовые предметы")]
        public List<Reward> exerciseRewards;
        public List<Requirement> exerciseRequirements;
        public List<ExerciseItem> exerciseItems;

        [Header("Задания, которые откроятся после завершения этого задания")]
        public List<FileExercise> newExercises;

        [Header("Настройка графического соправождения")]
        public string header;
        public bool isMail = false;
        public bool isRandom = false;
        [TextArea(10, 100)]
        public string description;
        public Sprite avatar;

        [HideInInspector] public bool isVisible = true;

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
