using UnityEngine;

/// <summary>
/// GroundSensor is a KLEP executable that detects whether the agent is grounded using a sphere cast.
/// When ground is detected, it pushes a specified key into the KLEPNeuron.
/// </summary>
public class GroundSensor : KLEPExecutableBase
{
    [Header("Ground Sensor Settings")]
    [Tooltip("Transform representing the sensor's position (usually at the agent's feet).")]
    public Transform sensorTransform;

    [Tooltip("Radius of the sphere for ground detection.")]
    public float detectionRadius = 0.5f;

    [Tooltip("Layer mask to specify what counts as ground.")]
    public LayerMask groundLayer;

    [Tooltip("Key to push when the agent is grounded.")]
    public string groundKey = "Ground_KEY";

    // Internal state to track if the agent is currently grounded
    private bool isGrounded;

    /// <summary>
    /// Initialization method called when the executable is set up.
    /// Validates necessary references and prepares the sensor.
    /// </summary>
    public override void Init()
    {
        base.Init();

        if (sensorTransform == null)
        {
            Debug.LogError("GroundSensor: Sensor Transform is not assigned.");
        }
    }

    /// <summary>
    /// Called every frame to update the sensor's status.
    /// Executes the ground check logic.
    /// </summary>
    public override void ExecutableUpdates()
    {
        if (sensorTransform == null || parentNeuron == null)
        {
            Debug.LogWarning("GroundSensor: Missing references (sensorTransform or parentNeuron).");
            return;
        }

        CheckGround();
    }

    /// <summary>
    /// Performs a sphere cast to detect if the sensor is in contact with the ground.
    /// If grounded, it triggers the key push to the neuron.
    /// </summary>
    private void CheckGround()
    {
        // Perform a sphere cast to detect ground within the specified radius and layer
        Collider[] colliders = Physics.OverlapSphere(sensorTransform.position, detectionRadius, groundLayer);

        // Determine if the sensor is grounded based on collider detection
        isGrounded = colliders.Length > 0;

        // Push the ground key if the sensor is grounded
        if (isGrounded)
        {
            PushGroundKey();
        }
    }

    /// <summary>
    /// Creates and pushes a key indicating that the sensor is grounded.
    /// The key is pushed to the parent neuron for processing within the KLEP system.
    /// </summary>
    private void PushGroundKey()
    {
        // Generate key data with a specified attraction value (1.0f)
        KeyCreationData keyData = KeyCreationService.CreateKeyData(groundKey, 1f, keyLoader);

        // Push the key to the neuron
        PushKey(keyData);
    }

    /// <summary>
    /// Visualizes the sensor's detection radius in the Unity editor using Gizmos.
    /// Displays the sphere in green if grounded, otherwise in red.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (sensorTransform != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(sensorTransform.position, detectionRadius);
        }
    }

    /// <summary>
    /// Determines whether the sensor has completed its function.
    /// Ground sensors are continuously active, so this always returns true.
    /// </summary>
    /// <returns>True, as the sensor does not have a completion state.</returns>
    public override bool IsComplete()
    {
        return true;
    }
}
