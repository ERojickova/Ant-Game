using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRole : MonoBehaviour
{
    public int activeRole = 0;

    private GUIStyle labelStyle;

    void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 32;
        labelStyle.normal.textColor = Color.white;
    }

    public void SetRoleFunction(int newRole)
    {
        activeRole = newRole;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200), "Role: " + activeRole, labelStyle);
    }
}
