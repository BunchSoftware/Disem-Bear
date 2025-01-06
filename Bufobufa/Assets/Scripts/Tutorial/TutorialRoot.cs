using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRoot : MonoBehaviour
{
    [SerializeField] private TableWithItems tableWithItems;
    [SerializeField] private PointerTutorialManager pointerTutorialManager;
    [SerializeField] private PostOfficeTube postOfficeTube;

    private DialogManager dialogManager;
    private Player player;

    public void Init(DialogManager dialogManager, Player player, SaveManager saveManager)
    {
        this.dialogManager = dialogManager;
        this.player = player;

        pointerTutorialManager.Init(dialogManager, player);
        tableWithItems.Init(saveManager);
    }
}
