using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(KLEPNeuron))]
public class KLEPNeuronEditor : Editor
{
    private KLEPNeuron kLEPNeuron;
    private List<string> heldKeysNames = new List<string>();
    private List<string> bufferedKeysNames = new List<string>();

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            kLEPNeuron = (KLEPNeuron)target;
            kLEPNeuron.bridge.RegisterEvent("AfterKeyBufferRelease", OnKeyBufferReleased);
            kLEPNeuron.bridge.RegisterEvent("BeforeCleanup", OnBeforeCleanup);
        }
    }

    private void OnDisable()
    {
        if (kLEPNeuron != null && Application.isPlaying)
        {
            kLEPNeuron.bridge.UnregisterEvent("AfterKeyBufferRelease", OnKeyBufferReleased);
            kLEPNeuron.bridge.UnregisterEvent("BeforeCleanup", OnBeforeCleanup);
        }
    }

    private void OnKeyBufferReleased(string eventName, object eventData)
    {
        UpdateHeldKeysDisplay();
        Repaint();
    }

    private void OnBeforeCleanup(string eventName, object eventData)
    {
        UpdateBufferKeysDisplay();
        Repaint();
    }

    private void UpdateHeldKeysDisplay()
    {
        heldKeysNames.Clear();
        if (kLEPNeuron.heldKeys != null)
        {
            foreach (KLEPKey key in kLEPNeuron.heldKeys)
            {
                heldKeysNames.Add(key.KeyName);                
            }
        }
    }

    void UpdateBufferKeysDisplay()
    {
        if (kLEPNeuron.keyManager != null && kLEPNeuron.keyManager.keyBuffer != null)
        {
            bufferedKeysNames.Clear();
            foreach (KLEPKey key in kLEPNeuron.keyManager.keyBuffer)
            {
                bufferedKeysNames.Add(key.KeyName);                                
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (kLEPNeuron != (KLEPNeuron)target)
        {
            OnDisable();
            OnEnable();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Held Keys", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        foreach (string keyName in heldKeysNames)
            EditorGUILayout.TextField("Key Name", keyName);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Buffered Keys", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        foreach (string keyName in bufferedKeysNames)
            EditorGUILayout.TextField("Buffered Key Name", keyName);
        EditorGUI.EndDisabledGroup();
    }
}