
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SLASHKeyManager : MonoBehaviour
{
    private KLEPNeuron parentNeuron;                                        // ref to the parent neuron
    public HashSet<KLEPKey> keyBuffer = new HashSet<KLEPKey>();             // the buffer of held keys, released at the start of each cycle    
    HashSet<KLEPKey> keyPool = new HashSet<KLEPKey>();
    [SerializeField] private int keyPoolValue = 0;

    List<SLASHkeyLoader> slashKeyLoaders = new List<SLASHkeyLoader>();

   

    private void Update()
    {
        keyPoolValue = keyPool.Count;
    }

    // Init the Key Manger
    public void Initialize(KLEPNeuron neuron)
    {
        parentNeuron = neuron; // Assigns the neuron

        // Load all SLASHkeyLoader assets from the specified path within the Resources directory
        slashKeyLoaders.Clear(); // Clear any existing loaders in case of re-initialization
        SLASHkeyLoader[] loadedKeyLoaders = Resources.LoadAll<SLASHkeyLoader>("KeyLoaders");
        slashKeyLoaders.AddRange(loadedKeyLoaders);

        // Ensure the directory path is correct and placed under a Resources folder.
        // For example, if your assets are located at "Assets/Resources/KeyLoaders", the path provided should be just "KeyLoaders".

        foreach (KLEPKey k in parentNeuron.ReturnInitialElements())
        {
           // Debug.Log("Checking property of " + k.KeyName);

            // Improved checking for MakeUnique property
            bool makeUnique = k.GetProperty<bool>(PropertyNames.MakeUnique.ToString(), "Slash Key Manager Init");

            if (makeUnique)
            {
               // Debug.Log(k.KeyName + " is marked as MakeUnique.");

                keyBuffer.Add(Clone(k)); // Add unique key to buffer
            }
            else
            {
               // Debug.Log(k.KeyName + " is not marked as MakeUnique or property doesn't exist.");
                keyBuffer.Add(k); // Non-unique key is added to buffer
            }
        }
    }

    // function that makes keys unique
    public KLEPKey Clone(KLEPKey k)
    {
        KLEPKey clone = ScriptableObject.CreateInstance<KLEPKey>();

        // Direct copy for static properties        
        clone.KeyName = k.KeyName;
        clone.Attractiveness = k.Attractiveness;

        // Create a new instance of PropertyManager for the clone
        clone.propertyManager = new PropertyManager();
        clone.propertyManager.properties = new List<PropertyDefinition>();
        clone.keyLoader = k.keyLoader;

        // Deep copy for dynamic properties
        foreach (var prop in k.propertyManager.properties)
        {
            PropertyDefinition copiedProp = new PropertyDefinition()
            {
                propertyNameEnum = prop.propertyNameEnum,
                propertyNameCustom = prop.propertyNameCustom,
                type = prop.type,
                Value = ClonePropertyValue(prop.Value) // Ensure Value is deep copied if necessary
            };

            clone.propertyManager.properties.Add(copiedProp);
        }
        return clone;
    }

    private object ClonePropertyValue(object value)
    {
        // Implement deep copy logic for the property value if necessary
        // For simple types like int, float, bool, etc., return the value directly
        // For complex types, create a new instance and copy the fields/properties

        if (value is ICloneable cloneable)
        {
            return cloneable.Clone();
        }

        // Handle other types if necessary
        return value;
    }    

    // calls bridge through the neuron to register needed events
    public void InitializeTriggers()
    {

        // Subscribe to key-related events
        parentNeuron.bridge.RegisterEvent("RequestKeyPushToBuffer", HandleKeyCreationEvent_PushToBuffer);
        parentNeuron.bridge.RegisterEvent("RequestKeyPushToNeuron", HandleKeyCreationEvent_PushToNeuron);
    
    }

    //*(* fix this please
    public SLASHkeyLoader FindBestMatchingKeyLoader(string keyName, List<PropertyDefinition> properties = null)
    {
        // Step 1: Direct support by key name
        SLASHkeyLoader directMatch = null; //<<------ *(* fix this please
        if (directMatch != null) return directMatch;

        // Step 2: Best property match
        SLASHkeyLoader bestPropertyMatch = null;
        int highestMatchScore = 0;
        foreach (var loader in slashKeyLoaders)
        {
            int matchScore = 0;

            if (properties == null)
                continue;

            foreach (var prop in properties)
            {
                if (loader.SupportsProperty(prop.PropertyName)) matchScore++;
            }

            if (matchScore > highestMatchScore)
            {
                highestMatchScore = matchScore;
                bestPropertyMatch = loader;
            }
        }

        // If a property match is found and it supports all properties or dynamic updates, use it
        if (bestPropertyMatch != null && (highestMatchScore == properties.Count || bestPropertyMatch.allowForeignKeys))
        {
            return bestPropertyMatch;
        }

        // Step 3: Fallback to loaders that allow dynamic property updates, if no ideal match is found
        var fallbackLoader = slashKeyLoaders.FirstOrDefault(loader => loader.allowForeignKeys);
        return fallbackLoader;
    }


    public bool TradeKey(string keyName, KLEPNeuron sourceNeuron, KLEPNeuron targetNeuron)
    {

        var keyToTrade = sourceNeuron.keyManager.GetFirstKeyInNeuronByName(keyName);

        if (keyToTrade != null)
        {
            sourceNeuron.RemoveKey(keyToTrade);     // Remove the key from the source neuron
            targetNeuron.AddKey(keyToTrade);        // Add the key to the target neuron
            return true;                            // Trade successful
        }

        return false;                               // Key not found or trade unsuccessful
    }

    public bool CopyKey(string keyName, KLEPNeuron sourceNeuron, KLEPNeuron targetNeuron)
    {

        var keyToCopy = sourceNeuron.keyManager.GetFirstKeyInNeuronByName(keyName);
        if (keyToCopy != null)
        {
            targetNeuron.AddKey(Clone(keyToCopy));  // Add the copied key to the target neuron
            return true;                            // Copy successful
        }
        return false;                               // Key not found or copy unsuccessful

    }

    // Handles immediate key creation and pushing to buffer
    public void HandleKeyCreationEvent_PushToBuffer(string eventName, object eventData)
    {
        if (eventData is KeyCreationData creationData)
        {
            IssueOrUpdateKeyToBuffer(creationData);
        }
        else Debug.LogError("We pushed non key data to push to buffer in keymanager");
    }

    // Handles immediate key creation and pushing to neuron
    public void HandleKeyCreationEvent_PushToNeuron(string eventName, object eventData)
    {
        if (eventData is KeyCreationData creationData)
        {
            KLEPKey key;

            // Try to get a key from the pool first
            if (keyPool.Count > 0)
            {
                key = keyPool.First(); // Get the first key available in the pool
                keyPool.Remove(key);   // Remove this key from the pool               
            }
            else
            {
                // If the pool is empty, create a new key
                key = CreateKey();
            }

            UpdateKeyProperties(key, creationData); // Update key properties based on creation data
            parentNeuron.AddKey(key); // Directly add to neuron for immediate processing            
        }
        else Debug.LogError("We pushed non key data to push to neuron in keymanager");
    }

    public KLEPKey GetFirstKeyInBufferByName(string keyName)
    {
        // Filter the keyBuffer list based on the provided key name
        return keyBuffer.FirstOrDefault(key => key.KeyName == keyName);
    }

    public KLEPKey GetFirstKeyInNeuronByName(string keyName)
    {
        return parentNeuron.heldKeys.FirstOrDefault(key => key.KeyName == keyName);

    }

    public KLEPKey GetFirstKeyInFixedKeysByName(string keyName)
    {
        return parentNeuron.fixedUpdateKeys.FirstOrDefault(key => key.KeyName == keyName);

    }

    public KLEPKey GetKey(string keyName) {
        KLEPKey klepKey = null;
        klepKey = parentNeuron.heldKeys.FirstOrDefault(key => key.KeyName == keyName);
        if (klepKey != null)
            return klepKey;
        else klepKey = keyBuffer.FirstOrDefault(key => key.KeyName == keyName);

        return klepKey;
    }

    // Common method to create or update a key based on provided data    
    public KLEPKey CreateKey()
    {
        KLEPKey k = ScriptableObject.CreateInstance<KLEPKey>();
        
        return k;
    }

    // Updates the properties of a key
    public void UpdateKeyProperties(KLEPKey key, KeyCreationData data)
    {

        if (data.expectedLoader == null)
        {
            Debug.LogError("Loader was null");
            return; // Exit if the loader is not properly set to prevent errors.
        }

        // Set basic attributes from the data
        key.KeyName = data.keyName;
        key.Attractiveness = data.attractiveness;        
        key.keyLoader = data.expectedLoader;

        // Initialize properties from the loader to ensure all necessary defaults are set
        key.InitializePropertiesFromLoader();

        // Now, iterate through each property in the KeyCreationData
        foreach (var newDataProp in data.Properties)
        {
            // Check if the property already exists in the key's PropertyManager
            var existingProp = key.propertyManager.properties.FirstOrDefault(p => p.PropertyName == newDataProp.PropertyName);

            if (existingProp != null)
            {
                // If the property exists, update it with new data
                existingProp.Value = newDataProp.Value;                
            }
            else
            {
                // If the property does not exist, add it (this handles dynamic or foreign properties)
                key.propertyManager.properties.Add(new PropertyDefinition
                {
                    propertyNameEnum = newDataProp.propertyNameEnum,
                    propertyNameCustom = newDataProp.propertyNameCustom,
                    type = newDataProp.type,
                    Value = newDataProp.Value
                });
            }
        }

    }

    public void IssueOrUpdateKeyToBuffer(KeyCreationData creationData)
    {
        KLEPKey key = keyBuffer.FirstOrDefault(k => k.KeyName == creationData.keyName);

        if (key == null)
        {
            // Try to get a key from the pool
            if (keyPool.Count > 0)
            {
                key = keyPool.First(); // Get the first key available in the pool
                keyPool.Remove(key);   // Remove this key from the pool                
            }
            else
            {
                // If the pool is empty, create a new key
                key = CreateKey();

            }
            UpdateKeyProperties(key, creationData);
            keyBuffer.Add(key); // Add the key to the buffer for this cycle
        }
        else
        {
            // If a key with the same name already exists in the buffer, just update its properties
            UpdateKeyProperties(key, creationData);
        }
    }

    public void RemoveKeyFromBuffer(string keyName)
    {
        var keyToRemove = keyBuffer.FirstOrDefault(k => k.KeyName == keyName);

        if (keyToRemove != null)
        {
            keyBuffer.Remove(keyToRemove);

        }

    }

    public void Cleanup()
    {        

        // Reset IsInUse to false for all keys immune to the key lifecycle
        foreach (var key in parentNeuron.heldKeys)
        {
            if (key.GetProperty<bool>(PropertyNames.ImmuneToKeyLifecycle.ToString(), "Cleanup function"))
            {
                // If a key is immune to the key lifecycle, ensure IsInUse is reset to false
                bool isInUse = key.GetProperty<bool>(PropertyNames.IsInUse.ToString(), "Cleanup function");
                if (isInUse)
                {
                    key.SetProperty(PropertyNames.IsInUse.ToString(), false, "Cleanup function");
                }
            }
            else { /*Debug.Log(" we will being cleaning up " + key.KeyName);*/ }
        }

        // Proceed with the existing cleanup logic for non-immune keys
        var keysToRemove = parentNeuron.heldKeys
                         .Where(key => !key.GetProperty<bool>(PropertyNames.ImmuneToKeyLifecycle.ToString(), nameof(Cleanup)))
                         .ToList();

        keysToRemove.ForEach(key =>
        {
            parentNeuron.RemoveKey(key); // Remove key from neuron
            ResetKeyProperties(key);     // Reset key properties for reuse
            keyPool.Add(key);            // Add key to the pool for reuse
        });
    }

    private void ResetKeyProperties(KLEPKey key)
    {
        if (key == null)
        {
            Debug.LogWarning("Attempted to reset properties of a null key or key with a null PropertyManager.");
            return;
        }
        key.propertyManager.properties.Clear();        

        // Reset other attributes of the key
        key.KeyName = "DefaultKeyName";
        key.Attractiveness = 0;
        
    }

    public void IssueBufferedKeys()
    {
        foreach (var key in keyBuffer)
        { parentNeuron.AddKey(key); } // Add the key from the buffer to the neuron
        keyBuffer.Clear();            // Clear the buffer
       
    }

}
