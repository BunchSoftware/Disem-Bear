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
    [SerializeField] private PickUpButton pickUpButton;
    [SerializeField] private MixButton mixButton;
    [SerializeField] private ClearButton clearButton;
    [SerializeField] private PostBox postBox;
    private ExerciseManager exerciseManager;
    private CheckTakeExercise takeExerciseConditions;
    private CheckGetAquariumCells getAquariumCellsConditions;
    private CheckCraftSomething craftSomethingConditions;
    private CheckOpenSomething checkOpenSomething;
    private CheckButtonsWorkbench checkButtonsWorkbench;
    private CheckPutObjectPostBox checkPutObjectPostBox;
    private CheckPlaceSomething checkPlaceSomething;
    private CheckPickUpObjectPostBox checkPickUpObjectPostBox;
    


    private DialogManager dialogManager;

    public void Init(DialogManager dialogManager)
    {
        this.dialogManager = dialogManager;

        exerciseManager = uIGameRoot.GetExerciseManager();
        takeExerciseConditions = GetComponent<CheckTakeExercise>();
        getAquariumCellsConditions = GetComponent<CheckGetAquariumCells>();
        craftSomethingConditions = GetComponent<CheckCraftSomething>();
        checkOpenSomething = GetComponent<CheckOpenSomething>();
        checkButtonsWorkbench = GetComponent<CheckButtonsWorkbench>();
        checkPutObjectPostBox = GetComponent<CheckPutObjectPostBox>();
        checkPlaceSomething = GetComponent<CheckPlaceSomething>();
        checkPickUpObjectPostBox = GetComponent<CheckPickUpObjectPostBox>();

        takeExerciseConditions.Init(dialogManager, exerciseManager, TV);
        getAquariumCellsConditions.Init(dialogManager, aquarium);
        craftSomethingConditions.Init(dialogManager, workbench);
        checkOpenSomething.Init(dialogManager);
        checkButtonsWorkbench.Init(pickUpButton, mixButton, clearButton, dialogManager);
        checkPutObjectPostBox.Init(postBox, dialogManager);
        checkPlaceSomething.Init(dialogManager);
        checkPickUpObjectPostBox.Init(postBox, dialogManager);
    }
}
