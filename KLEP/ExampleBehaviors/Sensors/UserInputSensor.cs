using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputEventType
{
    KeyDown,
    KeyUp,
    Continuous,
    HoldDuration,
    DoubleTap
}

[Serializable]
public class InputEvent
{
    public KeyCode key; // Key to detect (keyboard or mouse buttons)
    public InputEventType eventType; // Type of input to detect
    public float holdDurationThreshold = 1f; // Duration to track key held
    public float doubleTapThreshold = 0.25f; // Time to consider it a double-tap
}

public class UserInputSensor : KLEPExecutableBase
{
    [Header("Input Configuration")]
    public List<InputEvent> inputEvents = new List<InputEvent>(); // List of keys and events to track
    public LayerMask raycastLayerMask; // Layer mask for mouse interaction
    public string mouseDataKey = "MouseData_KEY"; // Consolidated mouse data key
    public string scrollDataKey = "MouseScroll_KEY"; // Scroll wheel data key

    private Camera mainCamera;

    // Dictionaries for tracking key states
    private Dictionary<KeyCode, float> lastKeyPressTime = new Dictionary<KeyCode, float>(); // For double-tap tracking
    private Dictionary<KeyCode, float> holdDuration = new Dictionary<KeyCode, float>(); // For hold duration tracking

    public override void Init()
    {
        base.Init();
        mainCamera = Camera.main;
    }

    public override void Execute()
    {
        if (parentNeuron == null || keyLoader == null)
        {
            Debug.LogError("UserInputSensor: Missing parentNeuron or keyLoader reference.");
            return;
        }
        
        HandleMouseInput();
        HandleScrollInput();
        HandleKeyboardInput();
    }

    /// <summary>
    /// Handles mouse input and pushes consolidated mouse data to the neuron.
    /// </summary>
    private void HandleMouseInput()
    {
        if (mainCamera == null) return;

        // Get mouse screen position
        Vector2 screenMousePosition = Input.mousePosition;

        // Ray from screen point
        Ray ray = mainCamera.ScreenPointToRay(screenMousePosition);

        // Get mouse world position (intersection with y = 0 plane)
        if (RayIntersectsPlane(ray, out Vector3 worldMousePosition))
        {
            // Push consolidated mouse data
            PushMouseData(screenMousePosition, worldMousePosition);
        }
    }

    /// <summary>
    /// Handles scroll wheel input and pushes scroll data.
    /// </summary>
    private void HandleScrollInput()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scrollDelta) > Mathf.Epsilon) // Only process if there's meaningful input
        {
            KeyCreationData scrollKey = KeyCreationService.CreateKeyData(scrollDataKey, 1f, keyLoader);
            scrollKey.SetProperty("ScrollDelta", scrollDelta);

            PushKey(scrollKey);

            Debug.Log($"UserInputSensor: Pushed Scroll Data - ScrollDelta = {scrollDelta}");
        }
    }

    /// <summary>
    /// Pushes mouse data as a single key with both screen and world positions.
    /// </summary>
    /// <param name="screenPosition">Mouse screen-space position</param>
    /// <param name="worldPosition">Mouse world-space position</param>
    private void PushMouseData(Vector2 screenPosition, Vector3 worldPosition)
    {
        KeyCreationData mouseData = KeyCreationService.CreateKeyData(mouseDataKey, 1f, keyLoader);
        mouseData.SetProperty("MouseScreenPosition", screenPosition);
        mouseData.SetProperty("MouseWorldPosition", worldPosition);

        PushKey(mouseData);
    }

    /// <summary>
    /// Handles keyboard input events based on the configured input events list.
    /// </summary>
    private void HandleKeyboardInput()
    {
        foreach (var inputEvent in inputEvents)
        {
            switch (inputEvent.eventType)
            {
                case InputEventType.KeyDown:
                    if (Input.GetKeyDown(inputEvent.key))
                        PushKeyData($"{inputEvent.key}_Down");
                    break;

                case InputEventType.KeyUp:
                    if (Input.GetKeyUp(inputEvent.key))
                        PushKeyData($"{inputEvent.key}_Up");
                    break;

                case InputEventType.Continuous:
                    if (Input.GetKey(inputEvent.key))
                        PushKeyData($"{inputEvent.key}_Hold");
                    break;

                case InputEventType.HoldDuration:
                    HandleHoldDuration(inputEvent.key, inputEvent.holdDurationThreshold);
                    break;

                case InputEventType.DoubleTap:
                    HandleDoubleTap(inputEvent.key, inputEvent.doubleTapThreshold);
                    break;
            }
        }
    }

    /// <summary>
    /// Handles hold duration events.
    /// </summary>
    private void HandleHoldDuration(KeyCode key, float threshold)
    {
        if (Input.GetKeyDown(key))
            holdDuration[key] = 0f;

        if (Input.GetKey(key))
        {
            if (!holdDuration.ContainsKey(key))
                holdDuration[key] = 0f;

            holdDuration[key] += Time.deltaTime;
            if (holdDuration[key] >= threshold)
            {
                PushKeyData($"{key}_Hold_{threshold}s");
                holdDuration[key] = 0f; // Reset to avoid spamming
            }
        }

        if (Input.GetKeyUp(key) && holdDuration.ContainsKey(key))
        {
            PushKeyData($"{key}_HeldFor_{holdDuration[key]}s");
            holdDuration.Remove(key);
        }
    }

    /// <summary>
    /// Handles double-tap events.
    /// </summary>
    private void HandleDoubleTap(KeyCode key, float threshold)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastKeyPressTime.ContainsKey(key) && Time.time - lastKeyPressTime[key] < threshold)
            {
                PushKeyData($"{key}_DoubleTap");
            }

            lastKeyPressTime[key] = Time.time;
        }
    }

    /// <summary>
    /// Pushes key data with an optional property.
    /// </summary>
    private void PushKeyData(string keyName, object value = null, string propertyName = null)
    {
        KeyCreationData keyData = KeyCreationService.CreateKeyData(keyName, 1f, keyLoader);

        if (value != null && propertyName != null)
        {
            keyData.SetProperty(propertyName, value);
        }

        PushKey(keyData);
    }

    /// <summary>
    /// Checks for the intersection of a ray with the horizontal plane at y = 0.
    /// </summary>
    private bool RayIntersectsPlane(Ray ray, out Vector3 intersectionPoint)
    {
        float planeY = 0f;

        if (Mathf.Abs(ray.direction.y) > Mathf.Epsilon) // Avoid division by zero
        {
            float t = (planeY - ray.origin.y) / ray.direction.y;
            if (t >= 0)
            {
                intersectionPoint = ray.origin + ray.direction * t;
                return true;
            }
        }

        intersectionPoint = Vector3.zero;
        return false;
    }
}
