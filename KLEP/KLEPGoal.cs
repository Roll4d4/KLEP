using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;

public class KLEPGoal : KLEPExecutableBase
{
    [SerializeField]
    private List<KlepExecutableLayer> executableLayers; // list of executable layers
    public int currentLayerIndex { get; private set; } = 0;                 // tracks the current layer to fire
    private bool isInitialized = false;                 // check to see if it is init

    public string activationKeyName;                    // optional name for an activation key to signal other exe's that this goal is firing
    public bool trackProgress = false;                  // Global boolean to track execution progress
    private bool failedToFire = false;                  // Flag to track if any executable fails to fire
    private bool oneFired = false;                      // Flag to track if any executable fails to fire    
    
    public bool activateDebugOnThisGoal = false;

    private void Awake()
    {
        int count = 0;
        foreach (var layer in executableLayers)         // Initialize all layers
        {
            layer.init(count);
            count++;
        }

    }

    public override void Execute()
    {
        if (!isInitialized)                             // check if we got to init
        {
            SubscribeToExecutableEvents();              // register to events
            isInitialized = true;
        }

        IssueActivationKey();                           // issue the activation key if need be

        if (parentNeuron.activateDebugLog && activateDebugOnThisGoal)
            Debug.Log("Starting goals exectuion");

        if (!trackProgress)                             // reset to layer index 0 if not tracking progress
            currentLayerIndex = 0;

        if (parentNeuron.activateDebugLog && activateDebugOnThisGoal)
            Debug.Log("index of " + currentLayerIndex +
                " Starting goals exectuion " + executableLayers.Count + " of layers");

        while (currentLayerIndex < executableLayers.Count)          // loop through layers
        {
            var currentLayer = executableLayers[currentLayerIndex]; // assign current layer           
            ExecuteLayerExecutables(currentLayer);                  // try to execute those in that layer
            if (parentNeuron.activateDebugLog && activateDebugOnThisGoal)
                Debug.DrawLine((Vector2)transform.position, ((Vector2)transform.position + Vector2.up + Vector2.left) * 50f, Color.white);

            bool canAdvance = EvaluateLayerCompletion(currentLayer, false);// see if we are allowed to advance to the next layer

            if (canAdvance)
            { currentLayerIndex++; }                                // increase to next layer
            else break;                                             // we cannot continue, so lets stop

        }

        if (!IsComplete())
            OnFailedToFireAllExecutables();

        if (currentLayerIndex >= executableLayers.Count)            // check if our index is > the layer count
        {  currentLayerIndex = 0; }                                 // if so, reset so we dont crash

    }

    public override void FixedExecute()
    {
        if (!isInitialized)                             // check if we got to init
        {
            SubscribeToExecutableEvents();              // register to events
            isInitialized = true;
        }

        if (!trackProgress)                             // reset to layer index 0 if not tracking progress
            currentLayerIndex = 0;

        while (currentLayerIndex < executableLayers.Count)          // loop through layers
        {
            var currentLayer = executableLayers[currentLayerIndex]; // assign current layer           
            ExecuteFixedUpdateLayerExecutables(currentLayer);       // try to execute those in that layer
            bool canAdvance = EvaluateLayerCompletion(currentLayer, true); // see if we are allowed to advance to the next layer

            if (canAdvance)
            { currentLayerIndex++; }                                // increase to next layer
            else break;                                             // we cannot continue, so let's stop

        }

        if (!IsComplete())
            OnFailedToFireAllExecutables();

        if (currentLayerIndex >= executableLayers.Count)            // check if our index is > the layer count
        { currentLayerIndex = 0; }                                 // if so, reset so we don't crash

    }

    public KLEPExecutableBase QueryGoalForSoloExe()
    {
                                                    // Iterate from the start up to the current layer index
        for (int i = 0; i <= currentLayerIndex; i++)
        {
            var layer = executableLayers[i];
            if (layer.IsCritical)
            {
                var soloExecutable = layer.ExecutablesInLayer.FirstOrDefault(exe => !exe.InTandem);
                if (soloExecutable != null && soloExecutable.CanValidate(parentNeuron.heldKeys) && soloExecutable.CanExecute(parentNeuron.heldKeys))
                    return soloExecutable;
            }
        }

                                                    // Check the current layer for any solo executable
        var currentLayerSoloExecutable = executableLayers[currentLayerIndex].ExecutablesInLayer
            .FirstOrDefault(exe => !exe.InTandem && exe.CanValidate(parentNeuron.heldKeys) && exe.CanExecute(parentNeuron.heldKeys));
        if (currentLayerSoloExecutable != null)
            return currentLayerSoloExecutable;

                                                    // Proceed to search in subsequent layers for a solo executable
        for (int i = currentLayerIndex + 1; i < executableLayers.Count; i++)
        {
            var layer = executableLayers[i];
            if (!executableLayers.Skip(currentLayerIndex + 1).Take(i - currentLayerIndex - 1).Any(l => l.ExecutionRequirement == KlepExecutableLayer.LayerExecutionRequirement.AllMustFire))
            {
                var soloExecutable = layer.ExecutablesInLayer.FirstOrDefault(exe => !exe.InTandem);
                if (soloExecutable != null && soloExecutable.CanValidate(parentNeuron.heldKeys) && soloExecutable.CanExecute(parentNeuron.heldKeys))
                    return soloExecutable;
            }
        }

                                                    // If no suitable executable is found,
        return this;                                // return the goal itself for consideration

    }

    // this pushes the activation key to the neuron
    private void IssueActivationKey()
    {
        string keyName = activationKeyName.NullIfEmpty();
        if (keyName == null)                        // Check if we have one, if not, we are done
            return;
       
        var bestMatchingLoader = parentNeuron.keyManager.FindBestMatchingKeyLoader(keyName);

        if (bestMatchingLoader == null)
        {
            Debug.LogWarning($"No suitable key loader found for {keyName}. Consider assigning a default loader.");
            return;
        }

                                                    // Creating KeyCreationData with the
                                                    // new approach and associated key loader        
                                                    // Push the key to the appropriate destination
        if (pushToNeuron)
            PushKeyToNeuron(KeyCreationService.CreateKeyData(activationKeyName,0f, bestMatchingLoader), executableName);
        if (pushToBuffer)
            PushKeyToBuffer(KeyCreationService.CreateKeyData(activationKeyName, 0f, bestMatchingLoader), executableName);

    }   

    // fire the exes in this layer
    private void ExecuteLayerExecutables(KlepExecutableLayer layer)
    {
        failedToFire = false;                                   // flag to see if there was a fail
        oneFired = false;                                       // flag to see if at least one fired
        foreach (var executable in layer.ExecutablesInLayer)
        {
            if (executable.CanValidate(parentNeuron.heldKeys)   // see if the exe is valid and can fire
                && executable.CanExecute(parentNeuron.heldKeys))
            {
                if (executable.isActiveAndEnabled)
                {
                    executable.ExecutableUpdates();
                    executable.Execute();                           // fire the exe
                }
                
                oneFired = true;                                // we fired at least one exe
            }
            else
            {
                failedToFire = true;                            // we failed to fire an exe
                if (parentNeuron.activateDebugLog && activateDebugOnThisGoal)
                    Debug.Log("we FAILED to fire " + executable.executableName);
            }
        }

    }

    private void ExecuteFixedUpdateLayerExecutables(KlepExecutableLayer layer)
    {
        failedToFire = false;                                   // flag to see if there was a fail
        oneFired = false;                                       // flag to see if at least one fired
        foreach (var executable in layer.ExecutablesInLayer)
        {
            if (executable.CanValidate(parentNeuron.heldKeys)   // see if the exe is valid and can fire
                && executable.CanExecute(parentNeuron.heldKeys))
            {
                if (executable.isActiveAndEnabled) { 
                    executable.ExecutableFixedUpdate();             // fire the fixed update exe
                    executable.FixedExecute();             // fire the fixed update exe
                }
                oneFired = true;                                // we fired at least one exe
            }
            else
            {
                failedToFire = true;                            // we failed to fire an exe
                if (parentNeuron.activateDebugLog && activateDebugOnThisGoal)
                    Debug.Log("we FAILED to fire " + executable.executableName);
            }
        }
    }

    // evaluation if we can move to the next layer
    private bool EvaluateLayerCompletion(KlepExecutableLayer layer, bool isFixedUpdate)
    {
        if (isFixedUpdate && !layer.ShouldFixedEXEContributeToOrder)
        {
            return true; // Skip evaluation if fixed update executables should not contribute
        }

        switch (layer.ExecutionRequirement)                     // check which requirement we use
        {
            case KlepExecutableLayer.LayerExecutionRequirement.AllMustFire:
                if (layer.PrioritizeCompletionStatus)
                {
                    // All must fire and complete
                    return !failedToFire && layer.ExecutablesInLayer.All(exe => exe.IsComplete());
                }
                else
                {
                    // All must fire, completion status is not prioritized
                    return !failedToFire;
                }
            case KlepExecutableLayer.LayerExecutionRequirement.AnyCanFire:
                if (layer.PrioritizeCompletionStatus)
                {
                    // Any can fire and must be complete
                    return layer.ExecutablesInLayer.Any(exe => exe.IsComplete());
                }
                else
                {
                    // Any can fire, completion status is not prioritized
                    return oneFired;
                }
            case KlepExecutableLayer.LayerExecutionRequirement.NoneNeedToFire:
                return true; // This layer doesn't need to fire executables to be considered complete
            default:
                return false;
        }
    }

    // inish to bridge to register its event
    private void SubscribeToExecutableEvents() 
    { parentNeuron.bridge.RegisterEvent("ExecutableCompleted", HandleExecutableCompletion); }

    // overridable method to handle completion
    public virtual void HandleExecutableCompletion(string eventName, object eventData)
    { /* Implementation depends on how you track completion events */  }

    // signify that the goal finished
    public override bool IsComplete()
    { return currentLayerIndex >= executableLayers.Count; }

    // overridable function to tell the system that the goal could not fire all layers
    public virtual void OnFailedToFireAllExecutables()
    {
        //parentNeuron.bridge.UniversalEvent("FailedToFireAllExecutable", null, "KLEPGoal");
    }

    public void ResetCurrentIndex() {
        currentLayerIndex = 0;
    }
}

[Serializable]
public class KlepExecutableLayer
{

    public enum LayerExecutionRequirement
    {
        AllMustFire,                // All executables in the layer must fire for the layer to be considered complete
        AnyCanFire,                 // Any executable firing in the layer is sufficient for moving on
        NoneNeedToFire              // This layer may not need to fire for the next layer to be considered
    }

    public int layerIndex;                      // the assigned layer index

    public List<KLEPExecutableBase> ExecutablesInLayer = 
        new List<KLEPExecutableBase>();         //the exe's in this layer

    public LayerExecutionRequirement ExecutionRequirement = 
        LayerExecutionRequirement.AllMustFire;  // the requirments to move to the next exe

    public bool IsCritical = false;             // Marks the layer as critical for execution all critcal
                                                // layers will fire in their order    

    public bool PrioritizeCompletionStatus = false; 
    public bool ShouldFixedEXEContributeToOrder = false;                                                // Governs whether to prioritize the IsComplete
                                                // status over merely firing an executable.
    public void init(int _layerIndex)
    {
        layerIndex = _layerIndex;
    }

}