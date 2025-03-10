using System.Collections;
using System.Collections.Generic;
using Game.LDialog;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;

public class MainConditionManager : MonoBehaviour
{
    [SerializeField] private UIGameRoot uIGameRoot;
    private ExerciseManager exerciseManager;
    private TakeExerciseConditions takeExerciseConditions;


    private DialogManager dialogManager;
    
    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;

        exerciseManager = uIGameRoot.GetExerciseManager();
        takeExerciseConditions = GetComponent<TakeExerciseConditions>();

        takeExerciseConditions.Init(dialogManager, exerciseManager);
    }
}
