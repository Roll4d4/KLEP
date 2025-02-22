using UnityEngine;

/// <summary>
/// ZombieMoveToEnemy_Router is a KLEP router that generates movement keys to move the agent (e.g., a zombie) toward a detected enemy.
/// The router listens for a "Mob_KEY" in the neuron's buffer, extracts the target's transform, and produces directional movement keys.
/// </summary>
public class ZombieMoveToEnemy_Router : KLEPExecutableBase
{
    [Header("Router Settings")]
    [Tooltip("Distance at which the zombie stops moving toward the target.")]
    public float stoppingDistance = 1.75f;

    [Tooltip("Time interval between key updates to avoid excessive key generation.")]
    public float updateInterval = 0.1f;

    // Current enemy target's transform
    private Transform targetTransform;

    // Key name to search for in the neuron's buffer
    private string mobKeyName = "Mob_KEY";

    // Timer to manage key update intervals
    private float nextUpdateTime = 0f;

    /// <summary>
    /// Called every frame to process the current target and generate movement keys if needed.
    /// Updates are rate-limited by the 'updateInterval' to optimize performance.
    /// </summary>
    public override void ExecutableUpdates()
    {
        // Limit updates to the specified interval
        if (Time.time >= nextUpdateTime)
        {
            ProcessTarget(); // Attempt to set the target transform from the Mob_KEY
            nextUpdateTime = Time.time + updateInterval;
        }

        // Generate movement keys if a valid target is set
        GenerateMovementKeys();
    }

    /// <summary>
    /// Retrieves the Mob_KEY from the neuron's buffer and extracts the target transform.
    /// If no valid Mob_KEY or target is found, resets the targetTransform.
    /// </summary>
    private void ProcessTarget()
    {
        // Attempt to retrieve the Mob_KEY from the neuron's buffer
        if (!parentNeuron.keyManager.GetKey(mobKeyName, out KLEPKey mobKey) || mobKey == null)
        {
            Debug.LogWarning("[ZombieMoveToEnemy_Router] No Mob_KEY found in the buffer.");
            targetTransform = null;
            return;
        }

        // Extract the target transform from the Mob_KEY properties
        targetTransform = mobKey.GetProperty<Transform>("Mob");

        if (targetTransform == null)
        {
            Debug.LogWarning("[ZombieMoveToEnemy_Router] Mob_KEY does not contain a valid target transform.");
        }
    }

    /// <summary>
    /// Generates movement keys based on the direction to the target.
    /// The router creates forward, backward, left, and right movement keys as well as a MouseData_KEY to face the target.
    /// </summary>
    private void GenerateMovementKeys()
    {
        if (targetTransform == null)
        {
            return; // No valid target to move toward
        }

        // Calculate the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

        // Stop generating movement keys if within the stopping distance
        if (distanceToTarget <= stoppingDistance)
        {
            return;
        }

        // Calculate the direction vector toward the target
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        // Generate movement keys based on the direction
        if (Vector3.Dot(direction, transform.forward) > 0.1f)
        {
            KeyCreationData forwardKey = KeyCreationService.CreateKeyData("W_Hold", 0, keyLoader);
            PushKey(forwardKey);
        }
        if (Vector3.Dot(direction, -transform.forward) > 0.1f)
        {
            KeyCreationData backwardKey = KeyCreationService.CreateKeyData("S_Hold", 0, keyLoader);
            PushKey(backwardKey);
        }
        if (Vector3.Dot(direction, transform.right) > 0.1f)
        {
            KeyCreationData rightKey = KeyCreationService.CreateKeyData("D_Hold", 0, keyLoader);
            PushKey(rightKey);
        }
        if (Vector3.Dot(direction, -transform.right) > 0.1f)
        {
            KeyCreationData leftKey = KeyCreationService.CreateKeyData("A_Hold", 0, keyLoader);
            PushKey(leftKey);
        }

        // Generate a MouseData_KEY to rotate the agent toward the target
        var mouseDataKey = KeyCreationService.CreateKeyData("MouseData_KEY", 0, keyLoader);
        mouseDataKey.SetProperty("MouseWorldPosition", targetTransform.position);
        PushKey(mouseDataKey);

        Debug.Log("[ZombieMoveToEnemy_Router] Generated movement keys toward the target.");
    }
}
