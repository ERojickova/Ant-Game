using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetRole : MonoBehaviour
{
    public AntRole activeRole = 0;

    public int numDiggerDown = 0;
    public int numDiggerSide = 0;
    public int numDiggerDiag = 0;
    public int numFloater = 0;
    public int numBlocker = 0;
    public int numClimber = 0;
    public int numBuilder = 0;
    public int numBomber = 0;

    public Button diggerDownButton;
    public Button diggerSideButton;
    public Button diggerDiagButton;
    public Button floaterButton;
    public Button blockerButton;
    public Button climberButton;
    public Button builderButton;
    public Button bomberButton;

    private Dictionary<AntRole, int> roleCounts;

    private GUIStyle labelStyle;

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

    private Role diggerDown;
    private Role diggerSide;
    private Role diggerDiag;
    private Role blocker;
    private Role floater;
    private Role builder;
    private Role climber;
    private Role bomber;

    void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 32;
        labelStyle.normal.textColor = Color.white;

        diggerDown = new Role(AntRole.DiggerDown, numDiggerDown, diggerDownButton, "Down digger: ");
        diggerSide = new Role(AntRole.DiggerSide, numDiggerSide, diggerSideButton, "Side digger: ");
        diggerDiag = new Role(AntRole.DiggerDiag, numDiggerDiag, diggerDiagButton, "Diag digger: ");
        blocker = new Role(AntRole.Blocker, numBlocker, blockerButton, "Blocker: ");
        floater = new Role(AntRole.Floater, numFloater, floaterButton, "Floater: ");
        builder = new Role(AntRole.Builder, numBuilder, builderButton, "Builder: ");
        climber = new Role(AntRole.Climber, numClimber, climberButton, "Climber: ");
        bomber = new Role(AntRole.Bomber, numBomber, bomberButton, "Bomber: ");
    }

    void Update()
    {
        UpdateButton(diggerDown);
        UpdateButton(diggerSide);
        UpdateButton(diggerDiag);
        UpdateButton(blocker);
        UpdateButton(floater);
        UpdateButton(builder);
        UpdateButton(climber);
        UpdateButton(bomber);
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
    }

    private bool isRoleAvailable(AntRole role)
    {
        return roleCounts.ContainsKey(role) && roleCounts[role] > 0;
    }
}
