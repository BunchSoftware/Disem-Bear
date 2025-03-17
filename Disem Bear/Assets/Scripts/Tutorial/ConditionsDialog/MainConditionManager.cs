using System.Collections;
using System.Collections.Generic;
using Game.Environment.Aquarium;
using Game.Environment.LMixTable;
using Game.LDialog;
using UI;
using UI.PlaneTablet.Exercise;
using UnityEngine;

public class MainConditionManager : MonoBehaviour
{
    [SerializeField] private UIGameRoot uIGameRoot;
    [SerializeField] private Aquarium aquarium;
    [SerializeField] private Workbench workbench;
    [SerializeField] private TV TV;
    private ExerciseManager exerciseManager;
    private CheckTakeExercise takeExerciseConditions;
    private CheckGetAquariumCells getAquariumCellsConditions;
    private CheckCraftSomething craftSomethingConditions;


    private DialogManager dialogManager;
    
    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;

        exerciseManager = uIGameRoot.GetExerciseManager();
        takeExerciseConditions = GetComponent<CheckTakeExercise>();
        getAquariumCellsConditions = GetComponent<CheckGetAquariumCells>();
        craftSomethingConditions = GetComponent<CheckCraftSomething>();

        takeExerciseConditions.Init(dialogManager, exerciseManager, TV);
        getAquariumCellsConditions.Init(dialogManager, aquarium);
        craftSomethingConditions.Init(dialogManager, workbench);
    }
}
