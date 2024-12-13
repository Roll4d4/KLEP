using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(SLASHkeyLoader))]
public class SLASHkeyLoaderEditor : Editor
{
    private GUIStyle headerStyle;

    public override void OnInspectorGUI()
    {
        SLASHkeyLoader keyLoader = (SLASHkeyLoader)target;

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                fixedHeight = 20,
                margin = new RectOffset(0, 0, 10, 10)
            };
        }

        DrawDefaultInspector(); // Draw the default inspector

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Properties", headerStyle);

        // Display existing properties
        if (keyLoader.properties != null)
        {
            for (int i = 0; i < keyLoader.properties.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                var property = keyLoader.properties[i];

                // Handling predefined property names
                property.propertyNameEnum = (PropertyNames)EditorGUILayout.EnumPopup("Predefined Property Name", property.propertyNameEnum);

                // Conditionally show custom property name field
                if (property.propertyNameEnum == PropertyNames.Custom)
                {
                    property.propertyNameCustom = EditorGUILayout.TextField("Custom Property Name", property.propertyNameCustom);
                }

                property.type = (PropertyType)EditorGUILayout.EnumPopup("Property Type", property.type);

                // Adjust the property value field based on the property type
                DrawPropertyValueField(property);

                // Move Up button
                if (GUILayout.Button("Move Up") && i > 0)
                {
                    Undo.RecordObject(keyLoader, "Move Property Up");
                    var temp = keyLoader.properties[i];
                    keyLoader.properties[i] = keyLoader.properties[i - 1];
                    keyLoader.properties[i - 1] = temp;
                }

                // Move Down button
                if (GUILayout.Button("Move Down") && i < keyLoader.properties.Count - 1)
                {
                    Undo.RecordObject(keyLoader, "Move Property Down");
                    var temp = keyLoader.properties[i];
                    keyLoader.properties[i] = keyLoader.properties[i + 1];
                    keyLoader.properties[i + 1] = temp;
                }

                // Remove button
                if (GUILayout.Button("Remove"))
                {
                    Undo.RecordObject(keyLoader, "Remove Property");
                    keyLoader.properties.RemoveAt(i);
                }
                EditorGUILayout.EndVertical();
            }
        }

        // Button to add a new default property
        if (GUILayout.Button("Add Default Property", GUILayout.Height(30)))
        {
            Undo.RecordObject(keyLoader, "Add Default Property");
            if (keyLoader.properties == null)
            {
                keyLoader.properties = new List<PropertyDefinition>();
            }
            keyLoader.properties.Add(new PropertyDefinition() { propertyNameEnum = PropertyNames.Custom, type = PropertyType.String, Value = "" });
            EditorUtility.SetDirty(keyLoader);
        }

        // Button to manually initialize default properties
        if (GUILayout.Button("Initialize Default Properties", GUILayout.Height(30)))
        {
            Undo.RecordObject(keyLoader, "Initialize Default Properties");
            keyLoader.InitializeDefaultProperties();
            EditorUtility.SetDirty(keyLoader);
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(keyLoader);
        }
    }

    private void DrawPropertyValueField(PropertyDefinition property)
    {
        // Use different fields based on the property type
        switch (property.type)
        {
            case PropertyType.Bool:
                property.Value = EditorGUILayout.Toggle("Value", Convert.ToBoolean(property.Value));
                break;
            case PropertyType.Int:
                property.Value = EditorGUILayout.IntField("Value", Convert.ToInt32(property.Value));
                break;
            case PropertyType.String:
                property.Value = EditorGUILayout.TextField("Value", Convert.ToString(property.Value));
                break;
            case PropertyType.Float:
                property.Value = EditorGUILayout.FloatField("Value", Convert.ToSingle(property.Value));
                break;
            // Add more cases as needed for your specific property types
            default:
                EditorGUILayout.LabelField("Unsupported property type for direct editing.");
                break;
        }
    }
}
