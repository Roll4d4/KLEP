using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum KeyEventType
{
    KeyDown,
    KeyUp,
    Continuous,
    DoubleTap,
    HoldDuration
}

[Serializable]
public class KeyEvent
{
    public KeyCode key;
    public KeyEventType eventType;
    public float doubleTapThreshold = 0.25f; // Time in seconds to consider it a double tap
    public float holdDurationThreshold = 1f; // Duration to track key held
}

public class S_Keyboard : KLEPExecutableBase
{
    public List<KeyEvent> keyEvents = new List<KeyEvent>();

    private Dictionary<KeyCode, float> keyLastPressTime = new Dictionary<KeyCode, float>();
    private Dictionary<KeyCode, float> keyHoldDuration = new Dictionary<KeyCode, float>();

    public bool MouseWheel = false;
    public GameEnums.KeyNames mouseZoomIn;
    public GameEnums.KeyNames mouseZoomOut;

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

        foreach (KeyEvent keyEvent in keyEvents)
        {
            CheckKeyEvent(keyEvent);
        }

        if (MouseWheel) PushMouseWheel();
    }

    private void CheckKeyEvent(KeyEvent keyEvent)
    {
        KeyCode key = keyEvent.key;
        KeyEventType eventType = keyEvent.eventType;

        switch (eventType)
        {
            case KeyEventType.KeyDown:
                if (Input.GetKeyDown(key))
                {
                    CreateAndPushKeyData(key.ToString() + "_Down_KEY");
                }
                break;

            case KeyEventType.KeyUp:
                if (Input.GetKeyUp(key))
                {
                    CreateAndPushKeyData(key.ToString() + "_Up_KEY");
                }
                break;

            case KeyEventType.Continuous:
                if (Input.GetKey(key))
                {
                    CreateAndPushKeyData(key.ToString() + "_KEY");
                }
                break;

            case KeyEventType.DoubleTap:
                if (Input.GetKeyDown(key))
                {
                    if (keyLastPressTime.ContainsKey(key) && Time.time - keyLastPressTime[key] < keyEvent.doubleTapThreshold)
                    {
                        CreateAndPushKeyData(key.ToString() + "_DoubleTap_KEY");
                    }
                    keyLastPressTime[key] = Time.time;
                }
                break;

            case KeyEventType.HoldDuration:
                if (Input.GetKeyDown(key))
                {
                    keyHoldDuration[key] = 0f;
                }
                if (Input.GetKey(key))
                {
                    keyHoldDuration[key] += Time.deltaTime;
                    if (keyHoldDuration[key] >= keyEvent.holdDurationThreshold)
                    {
                        CreateAndPushKeyData(key.ToString() + "_HoldDuration_" + keyEvent.holdDurationThreshold + "_KEY");
                        keyHoldDuration[key] = 0f; // Reset duration to avoid multiple events
                    }
                }
                if (Input.GetKeyUp(key))
                {
                    if (keyHoldDuration.ContainsKey(key) && keyHoldDuration[key] > 0)
                    {
                        CreateAndPushKeyData(key.ToString() + "_HeldFor_" + keyHoldDuration[key] + "_KEY");
                        keyHoldDuration.Remove(key);
                    }
                }
                break;
        }
    }

    void PushMouseWheel()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta != 0)
        {
            KeyCreationData zoomKeyData;
            if (scrollDelta > 0)
            {
                // Scroll delta positive: mouse wheel scrolled up
                zoomKeyData = KeyCreationService.CreateKeyData(mouseZoomIn.ToString(), 1, keyLoader);
                PushKey(zoomKeyData);                
            }
            else
            {
                // Scroll delta negative: mouse wheel scrolled down
                zoomKeyData = KeyCreationService.CreateKeyData(mouseZoomOut.ToString(), 1, keyLoader);
                PushKey(zoomKeyData);                
            }

            // Assuming HandleKeyCreationEvent_PushToNeuron is a method in your key manager that handles the key pushing
            parentNeuron.keyManager.HandleKeyCreationEvent_PushToNeuron("Zoom", zoomKeyData);
        }
    }

    private void CreateAndPushKeyData(string keyName)
    {
        // Create the KeyCreationData with the found loader and the prepared properties.
        KeyCreationData creationData = new KeyCreationData(keyName, 1f, keyLoader.properties, keyLoader);
        PushKey(creationData);
    }
}

//*******************************
// how to parse the held key data for that time value
//*******************************
/*private void ParseKeyData(string keyData, out string keyName, out float? holdDuration, out float? heldDuration)
{
    keyName = keyData;
    holdDuration = null;
    heldDuration = null;

    var matchHold = System.Text.RegularExpressions.Regex.Match(keyData, @"_HoldDuration_(\d+(\.\d+)?)_KEY$");
    var matchHeld = System.Text.RegularExpressions.Regex.Match(keyData, @"_HeldFor_(\d+(\.\d+)?)_KEY$");

    if (matchHold.Success)
    {
        keyName = keyData.Substring(0, matchHold.Index) + "_KEY";
        holdDuration = float.Parse(matchHold.Groups[1].Value);
    }
    else if (matchHeld.Success)
    {
        keyName = keyData.Substring(0, matchHeld.Index) + "_KEY";
        heldDuration = float.Parse(matchHeld.Groups[1].Value);
    }
}*/
