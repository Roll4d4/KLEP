using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeyLoader", menuName = "KLAP/KeyLoader")]
public class SLASHkeyLoader : ScriptableObject
{
    public List<KeyCreationData> keyTemplates;  // The templates of keys that this loader governs      
    public List<PropertyDefinition> properties; // The properties that all templates share
    public bool allowForeignKeys;                // Allows adding forgien keys to the templates *THIS PERSISTS PAST RUN TIME*

    private void OnEnable()
    {

        if (properties == null || properties.Count == 0)
        {

            InitializeDefaultProperties();  // Initialize only if properties are not already set
        }

        if (keyTemplates == null || keyTemplates.Count == 0)
        {
            keyTemplates = new List<KeyCreationData>();
            return;
        }
        initKCD();                          // Always organize Key Creation Data
    }

    void initKCD()
    {

        var propertiesList =
                properties.Select(p => new PropertyDefinition   // create a collection of all properties
                {
                    propertyNameEnum = p.propertyNameEnum,      // Assign Name if predefined name was selected
                    propertyNameCustom = p.propertyNameCustom,  // Assign Name if Custom name was used
                    type = p.type,                              // Assign the data type that this property expects
                    Value = p.Value                             // Directly pass object values
                }).ToList();

        foreach (var key in keyTemplates)
        {                     // Loop through all Key Templates

            key.Properties = propertiesList;
        }
    }

    public bool SupportsProperty(string propertyName)
    {
        // Check if the properties list contains a property with the given name
        // This checks both predefined and custom property names
        return properties.Any(propDef => propDef.PropertyName == propertyName);
    }

    public void InitializeDefaultProperties()
    {
        if (properties == null) properties = new List<PropertyDefinition>();

        // Example of adding a default property only if it doesn't exist
        AddPropertyIfMissing(PropertyNames.MakeUnique, PropertyType.Bool, false);
        AddPropertyIfMissing(PropertyNames.ImmuneToKeyLifecycle, PropertyType.Bool, false);
        AddPropertyIfMissing(PropertyNames.Positions, PropertyType.ListOfVector2, new List<Vector2>());
        AddPropertyIfMissing(PropertyNames.Transforms, PropertyType.ListOfTransformData, new List<Transform>());
        AddPropertyIfMissing(PropertyNames.IsInUse, PropertyType.Bool, false);
        AddPropertyIfMissing(PropertyNames.DateTime, PropertyType.String, DateTime.MinValue);

    }

    private void AddPropertyIfMissing(PropertyNames name, PropertyType type, object value)
    {
        if (!properties.Any(p => p.propertyNameEnum == name))
        {
            properties.Add(new PropertyDefinition
            {
                propertyNameEnum = name,
                type = type,
                Value = value
            });
        }
    }

    public bool AddKeyWithProperties(KeyCreationData newKeyData, List<PropertyDefinition> newProperties)
    {
        // Check and add properties first
        if (newProperties != null)
        {
            foreach (var newProp in newProperties)
            {
                if (!properties.Any(prop => prop.propertyNameEnum == newProp.propertyNameEnum && prop.propertyNameCustom == newProp.propertyNameCustom))
                {
                    properties.Add(new PropertyDefinition
                    {
                        propertyNameEnum = newProp.propertyNameEnum,
                        propertyNameCustom = newProp.propertyNameCustom,
                        type = newProp.type,
                        Value = newProp.Value
                    });
                }
            }
        }

        // Now, check and add the key
        if (!keyTemplates.Any(kt => kt.keyName == newKeyData.keyName))
        {
            keyTemplates.Add(newKeyData);
            return true;  // Indicates a new key with properties was added successfully
        }

        return false;  // Key already exists
    }

}

[System.Serializable]
public class KeyCreationData
{
    public string keyName;                                      // The name of the key to be
    public float attractiveness = 0;                            // The attraction of the key to be
    public SLASHkeyLoader expectedLoader;                       // the keyloader expected to govern this key to be

    [SerializeField] public List<PropertyDefinition> Properties { get; set; }    // List of properties this KCD will have

    public KeyCreationData(string name, float attractiveness, List<PropertyDefinition> properties, SLASHkeyLoader expectedLoader)
    {
        // Constructor to construct our Key Creation Data
        this.keyName = name;
        this.attractiveness = attractiveness;
        this.Properties = properties ?? new List<PropertyDefinition>();
        this.expectedLoader = expectedLoader;
    }

    public KeyCreationData()
    {
        // Ensure the Properties dictionary is initialized
        Properties = new List<PropertyDefinition>();
    }

    // This sets properties in the KCD (a key that is yet to be) -NOT- the key itself
    public void SetProperty<T>(string propertyName, T value, string sourceExecutable = "")
    {
        // Attempt to find an existing property by name
        var property = Properties.FirstOrDefault(p => p.PropertyName == propertyName);

        if (property != null)
        {
            // If found, update the existing property's value
            property.SetValue(value);

        }
        else if (expectedLoader.allowForeignKeys)
        {
            // If not found and foreign keys are allowed, create
            // a new PropertyDefinition
            property = new PropertyDefinition
            {
                // Depending on your system's requirements,
                // you might decide whether to use custom names
                // or another approach to naming dynamically added properties.

                propertyNameCustom = propertyName, // For dynamic properties, custom names are used
                                                   // The value is set via a dedicated method to ensure correct type handling
            };

            // Determine and set the PropertyType based on the type of T
            property.DeterminePropertyType<T>();
            property.SetValue(value);

            // Add the new property to the Properties list
            Properties.Add(property);
        }
        else
        {
            // Handle the case where the property doesn't exist,
            // and foreign keys are not allowed

            Debug.LogWarning($"{propertyName} was not found in definitions. Adding properties dynamically is not allowed.");
        }
    }

    // New method to explicitly return the property data
    public T GetProperty<T>(string propertyName)
    {
        var property = Properties.FirstOrDefault(p => p.PropertyName == propertyName);

        if (property != null)
        {
            return (T)Convert.ChangeType(property.Value, typeof(T));
        }
        else
        {
            Debug.LogWarning($"Property {propertyName} not found.");
            return default(T);
        }
    }

    public string CheckIfPropertyIsPresent(string propertyName)
    {
        Debug.Log("Reading Property for : " + keyName + " :: ");

        var property = Properties.FirstOrDefault(p => p.PropertyName == propertyName);

        if (property != null)
        {
            return $"Property Found: {propertyName} = {property.GetValue()}";
        }
        else
        {
            return $"Property {propertyName} not found.";
        }
    }

}

public static class KeyCreationService
{
    // Method to create a new KeyCreationData with basic setup
    public static KeyCreationData CreateKeyData(string keyName, float attractiveness, SLASHkeyLoader expectedLoader)
    {
        KeyCreationData kcd = new KeyCreationData
        {
            keyName = keyName,
            attractiveness = attractiveness,
            expectedLoader = expectedLoader
        };

        // Initialize properties from the loader
        InitializePropertiesFromLoader(kcd, expectedLoader);

        return kcd;
    }

    // Populate KeyCreationData directly using properties from SLASHkeyLoader
    private static void InitializePropertiesFromLoader(KeyCreationData kcd, SLASHkeyLoader loader)
    {
        if (loader == null)
        {
            Debug.LogError("Loader is null. Cannot initialize properties.");
            return;
        }

        // Directly assign the existing properties from the loader to the KeyCreationData
        kcd.Properties = loader.properties.Select(prop => new PropertyDefinition
        {
            propertyNameEnum = prop.propertyNameEnum,
            propertyNameCustom = prop.propertyNameCustom,
            type = prop.type,
            Value = prop.Value  // Copy the default value directly
        }).ToList();
    }


    // Add or update a specific property in a KeyCreationData
    public static void SetKeyProperty(KeyCreationData kcd, string propertyName, object value, PropertyType type)
    {
        var prop = kcd.Properties.FirstOrDefault(p => p.PropertyName == propertyName);
        if (prop != null)
        {
            prop.Value = value;
        }
        else
        {
            kcd.Properties.Add(new PropertyDefinition { propertyNameCustom = propertyName, Value = value, type = type });
        }
    }
}

