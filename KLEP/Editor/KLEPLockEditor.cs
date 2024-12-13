using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(KLEPLock))]
public class KLEPLockEditor : Editor
{
    private List<SLASHkeyLoader> availableKeyLoaders;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector
        KLEPLock lockScript = (KLEPLock)target;

        // Load all available SLASHkeyLoaders
        LoadAvailableKeyLoaders();

        // Draw selection for Key Loaders
        DrawKeyLoaderSelection(lockScript);

        // Ensuring all key names from selected loaders are gathered
        IEnumerable<string> allKeyNames = lockScript.keyLoader
            .SelectMany(loader => loader.keyTemplates.Select(kt => kt.keyName))
            .Distinct()
            .ToArray();

        // If there are no keys to display, show a message and return early
        if (!allKeyNames.Any())
        {
            EditorGUILayout.HelpBox("No keys available from selected SLASHkeyLoaders.", MessageType.Warning);
            return;
        }

        // Displaying and editing conditions
        DisplayAndEditConditions(lockScript, allKeyNames);

        // Apply changes
        ApplyChanges();
    }

    private void LoadAvailableKeyLoaders()
    {
        if (availableKeyLoaders == null || !availableKeyLoaders.Any())
        {
            availableKeyLoaders = new List<SLASHkeyLoader>(Resources.FindObjectsOfTypeAll<SLASHkeyLoader>());
        }
    }

    private void DrawKeyLoaderSelection(KLEPLock lockScript)
    {
        EditorGUILayout.LabelField("Key Loaders", EditorStyles.boldLabel);
        foreach (var keyLoader in availableKeyLoaders)
        {
            bool isSelected = lockScript.keyLoader.Contains(keyLoader);
            bool newIsSelected = EditorGUILayout.ToggleLeft(keyLoader.name, isSelected);
            if (newIsSelected != isSelected)
            {
                if (newIsSelected) lockScript.keyLoader.Add(keyLoader);
                else lockScript.keyLoader.Remove(keyLoader);
                EditorUtility.SetDirty(lockScript);
            }
        }
    }

    private void DisplayAndEditConditions(KLEPLock lockScript, IEnumerable<string> allKeyNames)
    {
        SerializedProperty conditionsProp = serializedObject.FindProperty("conditions");

        for (int i = 0; i < conditionsProp.arraySize; i++)
        {
            SerializedProperty conditionProp = conditionsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(conditionProp);

            // Key selection dropdown
            string currentKeyName = conditionProp.FindPropertyRelative("keyName").stringValue;
            int currentIndex = allKeyNames.ToList().IndexOf(currentKeyName);
            int selectedIndex = EditorGUILayout.Popup("Key", currentIndex, allKeyNames.ToArray());

            if (selectedIndex >= 0 && selectedIndex < allKeyNames.Count())
            {
                conditionProp.FindPropertyRelative("keyName").stringValue = allKeyNames.ElementAt(selectedIndex);
            }

            EditorGUILayout.BeginHorizontal();
            // Display other fields (keyMustExist, isOrCondition)
            EditorGUILayout.PropertyField(conditionProp.FindPropertyRelative("keyMustExist"), new GUIContent("Key Must Exist"));
            EditorGUILayout.PropertyField(conditionProp.FindPropertyRelative("isOrCondition"), new GUIContent("OR Condition"));

            // Remove condition button
            if (GUILayout.Button("Remove Condition", GUILayout.MaxWidth(150)))
            {
                conditionsProp.DeleteArrayElementAtIndex(i);
                // After removing an element, break out of the loop to prevent iterating over a modified collection
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Condition"))
        {
            conditionsProp.InsertArrayElementAtIndex(conditionsProp.arraySize);
        }
    }


    private void ApplyChanges()
    {
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        serializedObject.ApplyModifiedProperties();
    }
}