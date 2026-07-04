using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ComponentSwapper : EditorWindow
{
    private GameObject TargetGameObject;
    private GameObject NewGameObject;

    // Status track
    private string statusMessage = "";
    private bool isSuccess = false;

    [MenuItem("Awais/Component Swapper")]
    public static void ShowWindow()
    {
        GetWindow<ComponentSwapper>("Component Swapper");
    }


    void OnGUI(){
        // Title Header
        GUILayout.Space(12);
        GUILayout.BeginVertical("box");
        GUILayout.Space(5);
        var titleStyle = new GUIStyle(EditorStyles.boldLabel){
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("COMPONENT SWAPPER", titleStyle);
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Space(5);

        // Select GameObjects to swap
        GUILayout.Label("Select GameObjects to Swap", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.BeginVertical("box");
        GUILayout.Space(5);
        TargetGameObject = (GameObject)EditorGUILayout.ObjectField(
            "Target GameObject", TargetGameObject, typeof(GameObject), true);
        GUILayout.Space(5);
        var infoStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
        GUILayout.Label("This is the GameObject that will be replaced in the scene.", infoStyle);
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Space(5);

        GUILayout.BeginVertical("box");
        GUILayout.Space(5);
        NewGameObject = (GameObject)EditorGUILayout.ObjectField(
            "New GameObject", NewGameObject, typeof(GameObject), true);
        GUILayout.Space(5);
        GUILayout.Label("This is the GameObject that will be used to replace the target.", infoStyle);
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Space(12);

        // Swap Button
        bool bothAssigned = TargetGameObject != null && NewGameObject != null;
        // Button enable/disable
        GUI.enabled = bothAssigned;
        
        if (GUILayout.Button("Swap", GUILayout.Height(30)))
        {
            try{
            // Undo support
            Undo.RegisterCompleteObjectUndo(NewGameObject, "Swap GameObject");

            // Step 1 — Transform copy karo
            NewGameObject.transform.position = TargetGameObject.transform.position;
            NewGameObject.transform.rotation = TargetGameObject.transform.rotation;
            NewGameObject.transform.localScale = TargetGameObject.transform.localScale;

            // Step 2 — All component copy
            Component[] components = TargetGameObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                // Transform skip
                if (component is Transform) continue;

                    System.Type componentType = component.GetType();

                    // Check NewGameObject already has the component
                    Component newComp = NewGameObject.GetComponent(componentType);

                // add component if not present
                if (newComp == null)
                {
                    newComp = NewGameObject.AddComponent(componentType);
                }

                // Values copy
                EditorUtility.CopySerialized(component, newComp);
            }

            // Step 3 — Target hide
            TargetGameObject.SetActive(false);

            // Step 4 — New activate
            NewGameObject.SetActive(true);

            // Message set
                isSuccess = true;
                statusMessage = "Swap successfully complete! " + TargetGameObject.name + " → " + NewGameObject.name;
            }
            catch (System.Exception)
            {
                // Error message set
                isSuccess = false;
                statusMessage = "Swap Failed!";
            }
        }
        GUI.enabled = true;

        if(statusMessage != "")
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            // Success or failed style
            var statusStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                normal = {
                    textColor = isSuccess ? Color.green : Color.red
                },
                fontStyle = FontStyle.Bold
            };

            GUILayout.Label(statusMessage, statusStyle);
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(12);
        }   

    }
}