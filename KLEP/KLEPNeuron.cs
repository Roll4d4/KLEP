using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.tvOS;

public class KLEPNeuron : MonoBehaviour
{
    [SerializeField] public List<KLEPKey> initialKeys;            //Keys to Init the neuron with
    [SerializeField] public HashSet<KLEPKey> heldKeys;             //Current frames keys
    public HashSet<KLEPKey> fixedUpdateKeys; // List to hold keys for FixedUpdate
    public HashSet<KLEPExecutableBase> heldExecutables;               //all Execubales known to the neuron    


    public SLASHKeyManager keyManager;                             // global access to the key manager which manages keys
    public SLASHBridge bridge;                                     // global access to the bridge for the universal event system
    public KLEPAgent agent;                                        // global access to the agent for the decision making element of KLEP    

    public bool activateDebugLog = false;

    private void Awake()
    {

        heldKeys = new HashSet<KLEPKey>();                  // Set up the heldKeys
        fixedUpdateKeys = new HashSet<KLEPKey>(); // List to hold keys for FixedUpdate
        heldExecutables = new HashSet<KLEPExecutableBase>();   // Set up the held exe's
        InitializeComponents();                             // init and instantiate KLEP componets
    }

    private void InitializeComponents()
    {

        bridge = gameObject.AddComponent<SLASHBridge>();        // Add the bridge, no init is needed

        keyManager = gameObject.AddComponent<SLASHKeyManager>();// Add the keymanager
        keyManager.Initialize(this);                            // init The keymanger
        keyManager.InitializeTriggers();                        // set up the UES with bridge

        agent = gameObject.AddComponent<KLEPAgent>();           // add the agent
        agent.Initialize(this);                                 // init the agent
        agent.InitializeTriggers();                             // set up the UES with bridge


    }

    void Update()
    {
        bridge.UniversalEvent("BeforeCleanup");
        keyManager.Cleanup();
        
        UpdateCurrentFrameExecutables();
        keyManager.IssueBufferedKeys();        
        bridge.BridgeUpdate();
        agent.AgentUpdate();
        bridge.UniversalEvent("AfterKeyBufferRelease");
        fixedUpdateKeys = heldKeys;

    }

    private void FixedUpdate()
    {
        agent.AgentFixedUpdate(fixedUpdateKeys);
    }


    void UpdateCurrentFrameExecutables()
    {
        HashSet<KLEPExecutableBase> newFrameExecutables = new HashSet<KLEPExecutableBase>();
        GatherExecutables(transform, newFrameExecutables);
        SynchronizeExecutables(newFrameExecutables);
    }

    void SynchronizeExecutables(HashSet<KLEPExecutableBase> currentExecutables)
    {
        var currentHeldExecutables = new HashSet<KLEPExecutableBase>(heldExecutables);
        var toAdd = currentExecutables.Except(currentHeldExecutables).ToList();
        var toRemove = currentHeldExecutables.Except(currentExecutables).ToList();

        foreach (var executable in toRemove)
        {

            RemoveExecutable(executable);
        }

        foreach (var executable in toAdd)
        {

            AddExecutable(executable);
        }
    }


    void GatherExecutables(Transform parentTransform, HashSet<KLEPExecutableBase> executables)
    {
        foreach (Transform child in parentTransform)
        {
            var ke = child.GetComponent<KLEPExecutableBase>();
            if (ke != null && ke.enabled)
            {
                executables.Add(ke);
            }
            GatherExecutables(child, executables); // Recursively check children
        }
    }

    // this adds an executable to the neuron
    public void AddExecutable(KLEPExecutableBase executable)
    {
        if (!heldExecutables.Contains(executable) && !executable.IsManaged)
        {

            heldExecutables.Add(executable);                                    // add the new exe to known exe's                        
            executable.parentNeuron = this;                                     // give the executable a ref to this script            
            executable.RegisterExecutableEvents(bridge);                        // register all events that the executable may have            
            executable.Init();                                                  // call the init in the executable            
            executable.PopulateKeysAvailableAfterCompletion();                  // populates a field containing all keys the executable is expected to release
            bridge.UniversalEvent("ExecutableAdded", executable, "KLEPNeuron"); // call bridge to say we have a new executable
            executable.IsManaged = true;
        }
    }

    // This removes an executable from the neuron
    public void RemoveExecutable(KLEPExecutableBase executable)
    {

        if (executable != null && heldExecutables.Contains(executable) && executable.IsManaged)
        {
            executable.Cleanup();
            // any custom cleanup that an executable needs to do
            executable.UnregisterExecutableEvents(bridge);                          // Builtin call to unregister from bridge             
            heldExecutables.Remove(executable);                                     // removes the executable from the list
            bridge.UniversalEvent("ExecutableRemoved", executable, "KLEPNeuron");   // call bridge to say we removed an executable


            executable.parentNeuron = null;
            executable.IsManaged = false;
        }
    }

    // This uploads key to heldElements
    public void AddKey(KLEPKey key)
    {
        if (!heldKeys.Contains(key))                                // do we already have that key? - held keys are a hash
        {
            heldKeys.Add(key);                                      // add the key
                                                                    // bridge.UniversalEvent("KeyAdded", key, "KLEPNeuron");   // signal to bridge that we added a key
        }
    }

    // This removes a key from the heldElements
    public void RemoveKey(KLEPKey key)
    {
        if (heldKeys.Contains(key))
        {
            heldKeys.Remove(key);
        }

    }

    // Returns the initial set of elements (keys) held by the neuron.
    public List<KLEPKey> ReturnInitialElements()
    {
        return initialKeys;
    }

    public KLEPExecutableBase ReturnExecutable(string scriptTypeName)
    {
        return heldExecutables.FirstOrDefault(ex => ex.GetType().Name == scriptTypeName);
    }

    /*if (mobCTRL == null) // How to call this
        {
            var executable = parentNeuron.ReturnExecutable(nameof(G_MobCTRL));
            if (executable != null)
            {
                mobCTRL = executable as G_MobCTRL;
            } else mobCTRL = null;
        }*/
}