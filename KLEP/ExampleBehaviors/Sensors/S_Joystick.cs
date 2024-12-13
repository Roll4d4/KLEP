using System;
using System.Collections.Generic;
using UnityEngine;

public enum JoystickEventType
{
    KeyDown,
    KeyUp,
    Continuous
}

[Serializable]
public class JoystickInputEvent
{
    public string name;  // Name for the input event
    public string axisName;  // Name of the Unity Input axis
    public string keyName;  // Name of the KLEP key
    public bool isVector2;  // Whether the input is a Vector2 (e.g., joystick stick)
    public JoystickEventType eventType;  // Type of event
}

public class S_Joystick : KLEPExecutableBase
{
    public List<JoystickInputEvent> joystickInputs = new List<JoystickInputEvent>();

    private Dictionary<string, Vector2> lastVector2Values = new Dictionary<string, Vector2>();
    private Dictionary<string, float> lastFloatValues = new Dictionary<string, float>();

    public override bool IsComplete()
    {
        return true; // This sensor is always active and doesn't have a completion state
    }

    public override void ExecutableUpdates()
    {
        if (parentNeuron == null)
        {
            Debug.Log("Parent neuron is null.");
            return;
        }

        foreach (JoystickInputEvent inputEvent in joystickInputs)
        {
            CheckJoystickInput(inputEvent);
        }
    }

    private void CheckJoystickInput(JoystickInputEvent inputEvent)
    {
        if (inputEvent.isVector2)
        {
            Vector2 input = new Vector2(Input.GetAxis(inputEvent.axisName + "X"), Input.GetAxis(inputEvent.axisName + "Y"));
            HandleVector2Input(inputEvent, input);
        }
        else
        {
            float input = Input.GetAxis(inputEvent.axisName);
            HandleFloatInput(inputEvent, input);
        }
    }

    private void HandleVector2Input(JoystickInputEvent inputEvent, Vector2 input)
    {
        switch (inputEvent.eventType)
        {
            case JoystickEventType.KeyDown:
                if (input.magnitude > 0 && (!lastVector2Values.ContainsKey(inputEvent.keyName) || lastVector2Values[inputEvent.keyName] == Vector2.zero))
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;

            case JoystickEventType.KeyUp:
                if (input.magnitude == 0 && lastVector2Values.ContainsKey(inputEvent.keyName) && lastVector2Values[inputEvent.keyName] != Vector2.zero)
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;

            case JoystickEventType.Continuous:
                if (input.magnitude > 0)
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;
        }

        lastVector2Values[inputEvent.keyName] = input;
    }

    private void HandleFloatInput(JoystickInputEvent inputEvent, float input)
    {
        switch (inputEvent.eventType)
        {
            case JoystickEventType.KeyDown:
                if (Mathf.Abs(input) > 0.1f && (!lastFloatValues.ContainsKey(inputEvent.keyName) || Mathf.Abs(lastFloatValues[inputEvent.keyName]) <= 0.1f))
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;

            case JoystickEventType.KeyUp:
                if (Mathf.Abs(input) <= 0.1f && lastFloatValues.ContainsKey(inputEvent.keyName) && Mathf.Abs(lastFloatValues[inputEvent.keyName]) > 0.1f)
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;

            case JoystickEventType.Continuous:
                if (Mathf.Abs(input) > 0.1f)
                {
                    CreateAndPushKeyData(inputEvent.keyName, input);
                }
                break;
        }

        lastFloatValues[inputEvent.keyName] = input;
    }

    private void CreateAndPushKeyData(string keyName, Vector2 value)
    {
        KeyCreationData creationData = new KeyCreationData(keyName, 1f, keyLoader.properties, keyLoader);
        creationData.SetProperty("Value", value);
        PushKey(creationData);
    }

    private void CreateAndPushKeyData(string keyName, float value)
    {
        KeyCreationData creationData = new KeyCreationData(keyName, 1f, keyLoader.properties, keyLoader);
        creationData.SetProperty("Value", value);
        PushKey(creationData);
    }
}
