using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetRole : MonoBehaviour
{
    public AntRole activeRole = 0;

    public Button diggerDownButton;
    public Button diggerSideButton;
    public Button diggerDiagButton;
    public Button floaterButton;
    public Button blockerButton;
    public Button climberButton;
    public Button builderButton;
    public Button bomberButton;

    private GUIStyle labelStyle;
    private LevelData levelData;
    public int levelId;

    private class Role
    {
        public AntRole role;
        public int antCount;
        public Button button;
        public string buttonText;

        public Role(AntRole newRole, int newAntCount, Button newButton, string buttonText)
        {
            role = newRole;
            antCount = newAntCount;
            button = newButton;
            this.buttonText = buttonText;
        }
    }

    private Dictionary<AntRole, Role> roleDictionary;

    void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 32;
        labelStyle.normal.textColor = Color.white;

        levelData = System.Array.Find(GameManager.Instance.levelDataList.levels, level => level.levelId == levelId);

        roleDictionary = new Dictionary<AntRole, Role>
        {
            { AntRole.DiggerDown, new Role(AntRole.DiggerDown, levelData.numDiggerDown, diggerDownButton, "Down digger: ") },
            { AntRole.DiggerSide, new Role(AntRole.DiggerSide, levelData.numDiggerSide, diggerSideButton, "Side digger: ") },
            { AntRole.DiggerDiag, new Role(AntRole.DiggerDiag, levelData.numDiggerDiag, diggerDiagButton, "Diag digger: ") },
            { AntRole.Blocker, new Role(AntRole.Blocker, levelData.numBlocker, blockerButton, "Blocker: ") },
            { AntRole.Floater, new Role(AntRole.Floater, levelData.numFloater, floaterButton, "Floater: ") },
            { AntRole.Builder, new Role(AntRole.Builder, levelData.numBuilder, builderButton, "Builder: ") },
            { AntRole.Climber, new Role(AntRole.Climber, levelData.numClimber, climberButton, "Climber: ") },
            { AntRole.Bomber, new Role(AntRole.Bomber, levelData.numBomber, bomberButton, "Bomber: ") }
        };

    }

    void Update()
    {
        foreach(Role role in roleDictionary.Values)
        {
            UpdateButton(role);
        }

        if (activeRole == AntRole.None)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SetRoleFunction(int newRole)
    {
        AntRole newRoleEnum = (AntRole)newRole;
        if (isRoleAvailable(newRoleEnum))
        {
            if (activeRole != newRoleEnum)
            {
                activeRole = newRoleEnum;
            }
            else
            {
                activeRole = 0;
            }
        }
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200), "Role: " + activeRole, labelStyle);
    }

    private void UpdateButton(Role role)
    {
        TextMeshProUGUI buttonText = role.button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null )
        {
            buttonText.text = role.buttonText + role.antCount.ToString();
        }

        if (role.antCount <= 0)
        {
            role.button.interactable = false;
        }

        if (role.role == activeRole)
        {
            EventSystem.current.SetSelectedGameObject(role.button.gameObject);
        }
    }

    private bool isRoleAvailable(AntRole role)
    {
        return (roleDictionary.ContainsKey(role) ? roleDictionary[role].antCount : 0) > 0;
    }

    public void removedUsedAnt(AntRole antRole)
    {
        Role role = roleDictionary[antRole];
        role.antCount -= 1;
    }
}
