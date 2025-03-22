using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PlaneTablet.Exercise
{
    [Serializable]
    public class ExerciseRequirement
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
    [CreateAssetMenu(fileName = "Exercise", menuName = "Exercise/Exercise")]
    public class Exercise : ScriptableObject
    {
        public string nameExercise;
        [Header("Награды, требования, квестовые предметы")]
        public List<Reward> exerciseRewards;
        public List<ExerciseRequirement> exerciseRequirements;
        public List<ExerciseItem> exerciseItems;

        [Header("Задания, которые откроятся после завершения этого задания")]
        public List<Exercise> newExercises;

        [Header("Настройка графического соправождения")]
        public string header;
        public bool isMail = false;
        public bool isRandom = false;
        [TextArea(10, 100)]
        public string description;
        public Sprite avatar;

        [HideInInspector] public bool isVisible = true;
    }
}
