using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // For drawing Handles in the Unity Editor
#endif

/// <summary>
/// DetectNearbyMobs is a KLEP executable that scans for nearby mobs within a specified radius.
/// When mobs are detected, it creates and pushes Mob_KEYs to the neuron, enabling dynamic interaction with detected entities.
/// </summary>
public class DetectNearbyMobs : KLEPExecutableBase
{
    [Header("Detection Settings")]
    [Tooltip("Radius within which to detect nearby mobs.")]
    public float detectionRadius = 10f;

    [Tooltip("Layer mask to filter which objects are considered as mobs.")]
    private LayerMask mobLayer;

    // Cache detected neurons for visualization in the Unity editor
    private List<Transform> detectedNeurons = new List<Transform>();

    /// <summary>
    /// Initialization method called when the executable is set up.
    /// Sets the layer mask to detect objects on the "Mob" layer.
    /// </summary>
    public override void Init()
    {
        mobLayer = LayerMask.GetMask("Mob"); // Ensure the "Mob" layer is configured in the project
    }

    /// <summary>
    /// Execute method is called when the executable is triggered.
    /// It scans for nearby mobs, validates them, and pushes keys representing the detected mobs to the neuron.
    /// </summary>
    public override void Execute()
    {
        // Detect all nearby mobs within the specified radius and layer mask
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, mobLayer);

        // Clear previous detections for Gizmos visualization
        detectedNeurons.Clear();

        // List to store created Mob_KEYs for detected mobs
        List<KeyCreationData> detectedMobs = new List<KeyCreationData>();

        Debug.Log($"[DetectNearbyMobs] Found {colliders.Length} potential mobs.");

        foreach (Collider col in colliders)
        {
            Debug.Log($"{col.transform.name}: {col.gameObject.name}");
        }

        foreach (Collider collider in colliders)
        {
            Debug.Log($"Processing Collider: {collider.name}");

            // Attempt to find a KLEPNeuron component on the collider or its parent
            KLEPNeuron mob = collider.GetComponentInParent<KLEPNeuron>();

            if (mob == null)
            {
                Debug.LogWarning($"Collider {collider.name} does not have a KLEPNeuron component on itself or its parent.");
                continue;
            }

            // Avoid detecting the executing agent's own neuron
            if (mob.transform.GetInstanceID() == parentNeuron.transform.GetInstanceID())
            {
                Debug.Log("[DetectNearbyMobs] Skipping self.");
                continue;
            }

            // Attempt to retrieve the "Team_KEY" from the detected mob's key manager
            mob.keyManager.GetKey("Team_KEY", out KLEPKey mobTeam);

            if (mobTeam != null)
            {
                // Create a new Mob_KEY for the detected mob
                KeyCreationData mobKey = KeyCreationService.CreateKeyData("Mob_KEY", 0, keyLoader);
                detectedMobs.Add(mobKey);

                // Optionally add properties to the key (e.g., the mob's transform)
                mobKey.SetProperty("Mob", mob.transform);

                // Add to the list for Gizmos visualization
                detectedNeurons.Add(mob.transform);

                Debug.Log($"Detected mob with TeamID: {mobTeam.GetProperty<int>("TeamID")}");
            }
            else
            {
                Debug.LogWarning($"Mob {mob.name} does not have a TeamID key.");
            }
        }

        // Push all detected Mob_KEYs to the neuron
        foreach (var mobKey in detectedMobs)
        {
            PushKey(mobKey);
        }

        Debug.Log($"[DetectNearbyMobs] Released {detectedMobs.Count} Mob_KEYs to the neuron.");
    }

    /// <summary>
    /// Visualizes the detection radius and detected mobs in the Unity Editor using Gizmos and Handles.
    /// Draws a green circle for the detection radius and red lines and spheres for detected mobs.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw a flat circle for detection radius using Handles
#if UNITY_EDITOR
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, detectionRadius); // Flat circle on the Y-axis
#endif

        // Draw lines to detected neurons
        Gizmos.color = Color.red;
        foreach (var neuron in detectedNeurons)
        {
            if (neuron != null)
            {
                Gizmos.DrawLine(transform.position, neuron.position);
                Gizmos.DrawSphere(neuron.position + Vector3.up * 1.25f, 0.75f);
            }
        }
    }
}
