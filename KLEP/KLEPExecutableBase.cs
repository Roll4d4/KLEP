using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class KLEPExecutableBase : MonoBehaviour
{
                         // keys this executable releases
    public List<string> KeysAvailableAfterCompletionNames = new List<string>();
    [SerializeField]
    public SLASHkeyLoader keyLoader;
    public enum KlapType // enum to denote what type of executable it is, only used for debug purposes
    {
        Action,          // action effect the world based on keys
        Goal,            // goals may release an activation key and manage a sequence of keys
        Router,          // routers change one key (or no key) to another key
        Sensor           // sensors make keys based on the world around them
    }

    public KlapType executableType;                     // holds the executable type
    public KLEPNeuron parentNeuron;                     // ref to the parent Neuron
    public string executableName;                       // Name of the executable       
    public bool InTandem = true;                        // can this executable fire alonside other executables in frame?
    public bool IsManagedByGoal  = false;               // is this executable managed by a goal?
    public bool pushToNeuron = false;                   // push keys to neuron
    public bool pushToBuffer = true;                    // push keys to buffer
    [SerializeField]    
    public List<KLEPLock> LocksToValidateThisNode;      // Locks for validation
    [SerializeField]
    public List<KLEPLock> LocksToExecuteThisNode;       // Locks for execution
    public bool IsManaged { get; set; }

    void Awake()
    {
        // Replace non-unique locks with unique instances where required
        for (int i = 0; i < LocksToExecuteThisNode.Count; i++)
        {
            LocksToExecuteThisNode[i] = LocksToExecuteThisNode[i].CreateUniqueInstance();
        }

        // Replace non-unique locks with unique instances where required
        for (int i = 0; i < LocksToValidateThisNode.Count; i++)
        {
            LocksToValidateThisNode[i] = LocksToValidateThisNode[i].CreateUniqueInstance();
        }
        
    }

    public virtual void Init()
    {  /* can be overridden for unique init*/  }

    // Populates the field KeysAvailableAfterCompletion with the expected keys that this will release when completed
    virtual public List <string> PopulateKeysAvailableAfterCompletion() {
        return null;
    }

    // This is a centeralized hub for updates on executables.  A executable may run its own update,
    // however, this update only runs when the neuron fires it.  This ceneralizes the logic and timing
    // of our order of executions.    

    public virtual void OnEnterState() { }

    public virtual void OnExitState() { }

    public virtual void ExecutableFixedUpdate() {
        if (parentNeuron == null)
            return;
    }

    public virtual void FixedExecute()
    {
        if (parentNeuron == null)
            return;
    }

    public virtual void ExecutableUpdates()
    {
        if (parentNeuron == null)
            return;
        // This is an optional notification to bridge that we have entered update
        //  parentNeuron.bridge.UniversalEvent("ExecutableUpdateCycle", this, executableName);
    }

    public virtual void Execute()
    {
        if (parentNeuron == null)
            return;
        // This is an optional notification to bridge that we have entered execute
        //parentNeuron.bridge.UniversalEvent("ExecutableExecutionCycle", this, executableName);
    }

    public virtual bool CanValidate(HashSet<KLEPKey> heldKeys)
    {
                                        // Track if all locks are satisfied
        bool canValidate = true;
      //  Debug.Log(executableName + " has Lock Count to Validate = " + LocksToValidateThisNode.Count);
        foreach (var lockItem in LocksToValidateThisNode)
        {
          //  Debug.Log("Checking for validation by " + executableName);
          //  Debug.Log(lockItem.IsSatisfiedByAgent(heldKeys) + " is the result for " + lockItem.LockName);
            if (!lockItem.IsSatisfiedByAgent(heldKeys))
            {
             //   Debug.Log($"Checking keys for validation: {string.Join(", ", heldKeys.Select(k => k.KeyName))}");
                canValidate = false;
                break;                  // If any lock is not satisfied, break early

            }
            else
            {
                                        // If the lock is satisfied,
                                        // mark the keys that satisfied it as in use

                lockItem.conditions.Where(cond => cond.EvaluateCondition(heldKeys)).ToList().ForEach(cond =>
                {
                    var key = heldKeys.FirstOrDefault(k => k.KeyName == cond.ReturnKeyName());
                    if (key != null)
                    {
                        key.SetProperty(PropertyNames.IsInUse.ToString(), true, "CanValidate");
                    }
                });

            }
        }

        return canValidate;

    }

    public virtual bool CanExecute(HashSet<KLEPKey> heldKeys)
    {
                                        // Track if all locks are satisfied for execution
        bool canExecute = true;
       // Debug.Log(executableName + " has Lock Count to Execute = " + LocksToExecuteThisNode.Count);
        foreach (var lockItem in LocksToExecuteThisNode)
        {
          //  Debug.Log("Checking for execution by " + executableName);
          //  Debug.Log(lockItem.IsSatisfiedByAgent(heldKeys) + " is the result for " + lockItem.LockName);
            if (!lockItem.IsSatisfiedByAgent(heldKeys))
            {
              //  Debug.Log($"Checking keys for execution: {string.Join(", ", heldKeys.Select(k => k.KeyName))}");
                canExecute = false;
                break;                  // If any lock is not satisfied, break early

            }
            else
            {
                                        // If the lock is satisfied,
                                        // mark the keys that satisfied it as in use

                lockItem.conditions.Where(cond => cond.EvaluateCondition(heldKeys)).ToList().ForEach(cond =>
                {
                    var key = heldKeys.FirstOrDefault(k => k.KeyName == cond.ReturnKeyName());
                    if (key != null)
                    {
                        key.SetProperty(PropertyNames.IsInUse.ToString(), true, "CanExecute");
                    }
                });
            }
        }

        return canExecute;

    }

    public virtual bool IsComplete() { return true; }      
    
    public virtual void Cleanup()
    {  }

    // calcuated the attraction of the lock and the key that can validate it
    // only used for solo executables
    public float CalculateAttraction(HashSet<KLEPKey> elements)
    {
        float totalAttraction = 0f;

                                                    // Only proceed if the action can be validated and executed.                
        if (CanValidate(elements) && CanExecute(elements))
        {
                                                    // Iterate through validation locks
            foreach (var lockForValidation in LocksToValidateThisNode)
            {
                float lockAttraction = lockForValidation.Attractiveness;

                foreach (var condition in lockForValidation.conditions)
                {
                    KLEPKey prettiestKey = null;

                    foreach (var key in elements)   // find the most attractive key that satisfies the 
                    {                               // condition in case of many keys that solve for
                        if (condition.EvaluateCondition(new HashSet<KLEPKey> { key }))
                        {
                            if (prettiestKey == null || key.Attractiveness > prettiestKey.Attractiveness)
                            {
                                prettiestKey = key; //save the prettiest key, my precious
                            }
                        }
                    }

                    if (prettiestKey != null)
                    {                               //combine it to locks total
                        lockAttraction += prettiestKey.Attractiveness; 
                    }
                }

                totalAttraction += lockAttraction; //combine it to running total of executable
            }

                                                    // Iterate through execution locks;
                                                    // just as we did with validation locks

            foreach (var lockForExecution in LocksToExecuteThisNode)
            {

                float lockAttraction = lockForExecution.Attractiveness;

                                                    // go trough all conditions
                foreach (var condition in lockForExecution.conditions) 
                {
                    KLEPKey prettiestKey = null;

                    foreach (var key in elements)   // get the keys
                    {
                        if (condition.EvaluateCondition(new HashSet<KLEPKey> { key }))
                        {
                            if (prettiestKey == null || key.Attractiveness > prettiestKey.Attractiveness)
                            {                       // save the prettiest key
                                prettiestKey = key; 
                            }
                        }
                    }

                    if (prettiestKey != null)
                    {                               // add to lock
                        lockAttraction += prettiestKey.Attractiveness; 
                    }
                }

                totalAttraction += lockAttraction; // add to running total, as above
            }
        }

        return totalAttraction;
    }                                               //** NOTE ** 1 key to 1 condition,                                              

    public virtual void RegisterExecutableEvents(SLASHBridge bridge) {
        // Common event registrations for all executables
        // Call to a virtual method for registering custom events in derived classes
    }

    public virtual void UnregisterExecutableEvents(SLASHBridge bridge) {
        // Common event Unregistrations for all executables
        // Call to a virtual method for registering custom events in derived classes
    }

    public List<KLEPLock> GetAllLocks()
    {
        var allLocks = new List<KLEPLock>();
        allLocks.AddRange(LocksToValidateThisNode);
        allLocks.AddRange(LocksToExecuteThisNode.Distinct()); // Avoid duplicates
        return allLocks;
    }

    #region KeyHelperMethods
    // a helper method that will notify bridge to tell keymanger to issue a key to the neuron directly
    public void PushKeyToNeuron(KeyCreationData k, string s) { parentNeuron.bridge.UniversalEvent("RequestKeyPushToNeuron", k, s); }

    // a helper method that will notify bridge to tell keymanger to issue a key to the buffer for next frame
    public void PushKeyToBuffer(KeyCreationData k, string s) { parentNeuron.bridge.UniversalEvent("RequestKeyPushToBuffer", k, s); }

    public void PushKey(KeyCreationData k) { 
        if (pushToBuffer) { parentNeuron.bridge.UniversalEvent("RequestKeyPushToBuffer", k, executableName); }
        if (pushToNeuron) { parentNeuron.bridge.UniversalEvent("RequestKeyPushToNeuron", k, executableName); } 
    }

    public enum KeyCheckType { 
    inBuffer, inNeuron, fixedKey
    }

    public bool IsKeyPresent(string keyName, KeyCheckType type)
    {
        if (type == KeyCheckType.inBuffer) 
            return parentNeuron.keyManager.GetFirstKeyInBufferByName(keyName) != null;        
        if (type == KeyCheckType.fixedKey)
            return parentNeuron.keyManager.GetFirstKeyInFixedKeysByName(keyName) != null;
        return parentNeuron.keyManager.GetFirstKeyInNeuronByName(keyName) != null;
    }

    public KLEPKey GetKey(string keyName, KeyCheckType type) {
        if (type == KeyCheckType.inBuffer)
            return parentNeuron.keyManager.GetFirstKeyInBufferByName(keyName);
        if (type == KeyCheckType.fixedKey)
            return parentNeuron.keyManager.GetFirstKeyInFixedKeysByName(keyName);
        return parentNeuron.keyManager.GetFirstKeyInNeuronByName(keyName);
    }

    public KeyCreationData MakeKey(string keyName, float attraction) {
        return KeyCreationService.CreateKeyData(keyName, attraction, keyLoader);
    }

    public virtual string GetExecutableName()
    {
        return this.GetType().Name;
    }
    #endregion
}