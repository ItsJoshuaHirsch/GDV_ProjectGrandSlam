﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NextRoundButtonScript : MonoBehaviour {

    public GameObject resultMenu;
    public TextMeshProUGUI nextButtonText;
    LevelRestart levelRestart;
    RulesScript rulesScript;
    InGameGUI inGameGUI;

    void Start()
    {
        inGameGUI = FindObjectOfType<InGameGUI>();
        rulesScript = FindObjectOfType<RulesScript>();
        levelRestart = FindObjectOfType<LevelRestart>();
    }

    public void onClickNextButton()
    {
        Cursor.visible = false;
        levelRestart.levelRestartNextRound();
        FindObjectOfType<OverlayMethodenScript>().isInGame = true;

        if (nextButtonText.text == "Next Battle")
        {
            Debug.LogWarning("Reset Results");
            rulesScript.restartResults();
            inGameGUI.updateInGameGUIMultiplayer();
        }

        resultMenu.SetActive(false);

    }

}
