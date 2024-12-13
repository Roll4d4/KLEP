using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(KLEPKey))]
public class KLEPKeyEditor : Editor
{
    private List<SLASHkeyLoader> availableKeyLoaders;
    private string[] keyLoaderNames;
    private int selectedKeyLoaderIndex = 0;
    private string newTransformName = ""; // Temporary storage for new Transform name input
    private Vector2 newVector2Value = Vector2.zero; // Temporary storage for new Vector3 input

    private void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("t:SLASHkeyLoader", new[] { "Assets/Resources/KeyLoaders" });
        availableKeyLoaders = guids.Select(guid => AssetDatabase.LoadAssetAtPath<SLASHkeyLoader>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        keyLoaderNames = availableKeyLoaders.Select(kl => kl.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Key Properties", EditorStyles.boldLabel);

        KLEPKey key = (KLEPKey)target;

        // Load KeyLoader selection dropdown
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("KeyLoader", EditorStyles.boldLabel);
        selectedKeyLoaderIndex = EditorGUILayout.Popup("Select KeyLoader", selectedKeyLoaderIndex, keyLoaderNames);

        if (GUILayout.Button("Add to KeyLoader"))
        {
            AddKeyToSelectedKeyLoader();
        }

        // Property editor
        DrawPropertyEditor(key);

        // Button for synchronization
        
        // Buttons for additional functionality
        EditorGUI.BeginDisabledGroup(key.keyLoader == null);
        if (GUILayout.Button("Synchronize Properties with KeyLoader"))
        {
            key.SynchronizePropertiesWithLoader();
            key.InitializePropertiesFromLoader();
            MarkDirty(key);
        }

        EditorGUI.EndDisabledGroup();

        // Apply changes to ensure they are serialized
        if (GUI.changed)
        {
            MarkDirty(key);
        }
    }
    private void DrawPropertyEditor(KLEPKey key)
    {
        if (key.propertyManager.properties == null) return;

        foreach (var property in key.propertyManager.properties)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(property.PropertyName, GUILayout.MaxWidth(200));

            // Handle different types based on property.type
            switch (property.type)
            {
                case PropertyType.Bool:
                    // Directly cast the value to bool for editing if it's already a bool
                    if (property.Value is bool boolVal)
                    {
                        boolVal = EditorGUILayout.Toggle("Value", boolVal);
                        property.Value = boolVal;
                    }
                    break;
                case PropertyType.Int:
                    // Similar direct cast for int
                    if (property.Value is int intVal)
                    {
                        intVal = EditorGUILayout.IntField("Value", intVal);
                        property.Value = intVal;
                    }
                    break;
                case PropertyType.String:
                    // Direct editing for strings
                    if (property.Value is string stringVal)
                    {
                        stringVal = EditorGUILayout.TextField("Value", stringVal);
                        property.Value = stringVal;
                    }
                    break;
                case PropertyType.Float:
                    if (property.Value is float floatVal) {
                        floatVal = EditorGUILayout.FloatField("Value", floatVal);
                        property.Value = floatVal;
                    }
                    break;
                case PropertyType.DateTime:
                    // Assuming property.Value already contains a DateTime or null
                    string dateTimeStr = property.Value != null ? ((DateTime)property.Value).ToString("yyyy-MM-dd") : "";
                    dateTimeStr = EditorGUILayout.TextField("Value (yyyy-MM-dd)", dateTimeStr);
                    if (DateTime.TryParse(dateTimeStr, out DateTime dateTimeVal))
                    {
                        property.Value = dateTimeVal;
                    }
                    break;
                case PropertyType.ListOfVector2: // Assuming you have a Vector3List type or use ListOfVector2 with Vector3 values
                                               // Display current list (simplified for brevity)
                    EditorGUILayout.LabelField("Current Vectors:");
                    var vectorList = property.Value as List<Vector3>; // Adjust to Vector2 if necessary
                    if (vectorList != null)
                    {
                        foreach (var vector in vectorList)
                        {
                            EditorGUILayout.LabelField($" - {vector}");
                        }
                    }

                    // Input fields for new Vector3 value
                    newVector2Value = EditorGUILayout.Vector3Field("New Vector3 Value", newVector2Value);

                    // Button to add the new Vector3 to the list
                    if (GUILayout.Button("Add Vector3"))
                    {
                        if (vectorList == null) property.Value = new List<Vector3>(); // Initialize the list if null
                        (property.Value as List<Vector3>)?.Add(newVector2Value);
                        newVector2Value = Vector3.zero; // Reset temporary value
                    }
                    break;
                case PropertyType.ListOfTransformData: // Assuming you define this type for storing Transform names
                                                     // Display current list (simplified for brevity)
                    EditorGUILayout.LabelField("Current Transforms:");
                    var transformNameList = property.Value as List<string>;
                    if (transformNameList != null)
                    {
                        foreach (var name in transformNameList)
                        {
                            EditorGUILayout.LabelField($" - {name}");
                        }
                    }

                    // Input field for new Transform name
                    newTransformName = EditorGUILayout.TextField("New Transform Name", newTransformName);

                    // Button to add the new Transform name to the list
                    if (GUILayout.Button("Add Transform Name"))
                    {
                        if (transformNameList == null) property.Value = new List<string>(); // Initialize the list if null
                        (property.Value as List<string>)?.Add(newTransformName);
                        newTransformName = ""; // Reset temporary value
                    }
                    break;
                // Extend handling for other types as needed
                default:
                    EditorGUILayout.LabelField($"Unsupported property type: {property.type}");
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void AddKeyToSelectedKeyLoader()
    {
        KLEPKey key = (KLEPKey)target;
        if (availableKeyLoaders.Count > selectedKeyLoaderIndex)
        {
            SLASHkeyLoader selectedLoader = availableKeyLoaders[selectedKeyLoaderIndex];
            // Use the AddKeyIfNotExists method or equivalent logic here
            // Ensure you handle key data properly
        }
    }

    private void MarkDirty(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [System.Serializable]
    public class SerializableVector2List
    {
        public List<Vector2> list;

        public SerializableVector2List(List<Vector2> list)
        {
            this.list = list;
        }
    }

}
