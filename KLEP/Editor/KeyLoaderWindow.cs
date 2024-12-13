using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class KeyLoaderWindow : EditorWindow
{
    private Vector2 loaderScrollPos; // Scroll position for the KeyLoaders column
    private Vector2 propertyScrollPos; // Scroll position for the properties column
    private Vector2 allKeysScrollPos; // Scroll position for the All Keys column
    private List<SLASHkeyLoader> keyLoaders;
    private SLASHkeyLoader selectedKeyLoader;
    private Dictionary<string, List<SLASHkeyLoader>> allKeys;

    [MenuItem("Tools/KeyLoader Viewer")]
    public static void ShowWindow()
    {
        GetWindow<KeyLoaderWindow>("KeyLoader Viewer");
    }

    private void OnEnable()
    {
        keyLoaders = Resources.FindObjectsOfTypeAll<SLASHkeyLoader>().ToList();
        GatherAllKeys();
    }

    private void GatherAllKeys()
    {
        allKeys = new Dictionary<string, List<SLASHkeyLoader>>();
        foreach (var loader in keyLoaders)
        {
            foreach (var key in loader.keyTemplates)
            {
                if (!allKeys.ContainsKey(key.keyName))
                {
                    allKeys[key.keyName] = new List<SLASHkeyLoader>();
                }
                allKeys[key.keyName].Add(loader);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Column for KeyLoaders
        loaderScrollPos = EditorGUILayout.BeginScrollView(loaderScrollPos, GUILayout.Width(position.width / 3), GUILayout.Height(position.height));
        if (keyLoaders != null)
        {
            foreach (var loader in keyLoaders)
            {
                if (GUILayout.Button(loader.name))
                {
                    selectedKeyLoader = loader;
                }
            }
        }
        EditorGUILayout.EndScrollView();

        // Column for properties of the selected KeyLoader
        propertyScrollPos = EditorGUILayout.BeginScrollView(propertyScrollPos, GUILayout.Width(position.width / 3), GUILayout.Height(position.height));
        if (selectedKeyLoader != null)
        {
            DrawProperties(selectedKeyLoader);
        }
        EditorGUILayout.EndScrollView();

        // Column for all keys
        allKeysScrollPos = EditorGUILayout.BeginScrollView(allKeysScrollPos, GUILayout.Width(position.width / 3), GUILayout.Height(position.height));
        DrawAllKeys();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawProperties(SLASHkeyLoader keyLoader)
    {
        EditorGUILayout.LabelField("Properties for: " + keyLoader.name, EditorStyles.boldLabel);
        foreach (var key in keyLoader.keyTemplates)
        {
            EditorGUILayout.LabelField("Key: " + key.keyName);
            foreach (var prop in key.Properties.Where(p => p.propertyNameCustom != ""))
            {
                EditorGUILayout.TextField(prop.propertyNameCustom, prop.Value.ToString());
            }
        }
    }

    private void DrawAllKeys()
    {
        EditorGUILayout.LabelField("All Keys", EditorStyles.boldLabel);
        if (allKeys != null)
        {
            foreach (var key in allKeys)
            {
                EditorGUILayout.LabelField("Key: " + key.Key);
                foreach (var loader in key.Value)
                {
                    EditorGUILayout.LabelField("Defined in: " + loader.name);
                }
            }
        }
    }
}
