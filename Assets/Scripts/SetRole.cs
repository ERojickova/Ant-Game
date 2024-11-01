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


    private GUIStyle labelStyle;

    void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 32;
        labelStyle.normal.textColor = Color.white;
    }

    void Update()
    {
        UpdateButtonText(diggerDownButton, "Down digger: ", numDiggerDown);
        UpdateButtonText(diggerSideButton, "Side digger: ", numDiggerSide);
        UpdateButtonText(diggerDiagButton, "Diag digger: ", numDiggerDiag);
        UpdateButtonText(floaterButton, "Floater: ", numFloater);
        UpdateButtonText(blockerButton, "Blocker: ", numBlocker);
        UpdateButtonText(climberButton, "Climber: ", numClimber);
        UpdateButtonText(builderButton, "Builder: ", numBuilder);
        UpdateButtonText(bomberButton, "Bomber: ", numBomber);
    }

    public void SetRoleFunction(int newRole)
    {
        AntRole newRoleEnum = (AntRole)newRole;
        
        if (activeRole != newRoleEnum)
        {
            activeRole = newRoleEnum;
        }
        else
        {
            activeRole = 0;
        }
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200), "Role: " + activeRole, labelStyle);
    }

    private void UpdateButtonText(Button button, string prefix, int numberOfAnts)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null )
        {
            buttonText.text = prefix + numberOfAnts.ToString();

        }
    }
}
