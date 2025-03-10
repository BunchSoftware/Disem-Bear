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
    private ExerciseManager exerciseManager;
    private TakeExerciseConditions takeExerciseConditions;
    private GetAquariumCellsConditions getAquariumCellsConditions;
    private CraftSomethingConditions craftSomethingConditions;


    private DialogManager dialogManager;
    
    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;

        exerciseManager = uIGameRoot.GetExerciseManager();
        takeExerciseConditions = GetComponent<TakeExerciseConditions>();
        getAquariumCellsConditions = GetComponent<GetAquariumCellsConditions>();
        craftSomethingConditions = GetComponent<CraftSomethingConditions>();

        takeExerciseConditions.Init(dialogManager, exerciseManager);
        getAquariumCellsConditions.Init(dialogManager, aquarium);
        craftSomethingConditions.Init(dialogManager, workbench);
    }
}
