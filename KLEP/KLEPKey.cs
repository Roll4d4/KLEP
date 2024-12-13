using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewKey", menuName = "KLAP/Key")]
public class KLEPKey : ScriptableObject
{
    // Manages properties for this key, including default values and custom data
    [SerializeField] public PropertyManager propertyManager = new PropertyManager();

    // The name identifier for the key
    [SerializeField] public string KeyName;

    // A value indicating how attractive or important this key is, potentially used for sorting or prioritization
    [SerializeField] public float Attractiveness;

    // Reference to the key loader that provides the default properties for this key
    public SLASHkeyLoader keyLoader;

    // Tracks the ID of the last key loader used, useful for re-initializing or updates
    public string lastUsedKeyLoaderID;

    private void OnEnable()
    {

        // Ensure the PropertyManager is properly initialized to prevent null reference errors
        if (propertyManager == null)
        {
            Debug.LogError("PropertyManager is not initialized.");
            return;
        }

        // Assign the keyLoader to the propertyManager, if it's not already set,
        // ensuring properties can be managed correctly
        if (propertyManager.keyLoader == null)
        {
            propertyManager.keyLoader = keyLoader;
        }

        // Initialize key properties from the loader if there's no custom data defined,
        // ensuring the key has the necessary default properties
        if (keyLoader != null && propertyManager.keyLoader == keyLoader)
        {                                 // Initialize properties if no custom data is present
            if (!propertyManager.HasCustomData())
            { InitializePropertiesFromLoader(); }
            else
            { /*Debug.Log(KeyName + " key was not initialized because it has custom data"); */}
        }

    }

    public void SetProperty<T>(string propertyName, T value, string sourceExecutable = "")
    {
        // Delegate property setting to the property manager,
        // abstracting away the implementation details
        propertyManager.SetProperty(propertyName, value);
    }

    public T GetProperty<T>(string propertyName, string sourceExecutable = "")
    {
        // Retrieve a property's value through the property manager,
        // abstracting away the implementation details
        return propertyManager.GetProperty<T>(propertyName);
    }

    // Initializes properties for this key using the definitions from the assigned key loader
    public void InitializePropertiesFromLoader()
    {
        if (keyLoader == null) return;  // Do nothing if no keyLoader is assigned

        // Loop through properties defined in the keyLoader
        foreach (var propDef in keyLoader.properties)
        {
            var existingProperty = propertyManager.properties.FirstOrDefault(p => p.PropertyName == propDef.PropertyName);

            if (existingProperty != null)
            {
                // Check if the existing property has a non-default value
                if (!IsDefaultValue(existingProperty.Value, propDef))
                {
                    continue; // Skip overwriting if a new value is present
                }
                else
                {
                    existingProperty.Value = propDef.Value; // Update with default value if it's still default
                }
            }
            else
            {
                // Add new property if it doesn't exist
                propertyManager.properties.Add(new PropertyDefinition
                {
                    propertyNameEnum = propDef.propertyNameEnum,
                    propertyNameCustom = propDef.propertyNameCustom,
                    type = propDef.type,
                    Value = propDef.Value
                });
            }
        }
    }

    public bool HasProperty(string propertyName)
    {
        // Check if the property manager contains a property with the specified name
        return propertyManager.properties.Any(p => p.PropertyName == propertyName);
    }

    public void ClearProperty(string propertyName)
    {
        // Attempt to find the property in the property manager
        var property = propertyManager.properties.FirstOrDefault(p => p.PropertyName == propertyName);

        if (property != null)
        {
            // Check if a default value exists in the keyLoader
            var defaultProperty = keyLoader?.properties.FirstOrDefault(p => p.PropertyName == propertyName);

            if (defaultProperty != null)
            {
                // Reset the property to the default value if it exists
                property.Value = defaultProperty.Value;
                Debug.Log($"Property '{propertyName}' reset to default value.");
            }
            else
            {
                // Remove the property if no default value is specified
                propertyManager.properties.Remove(property);
                Debug.Log($"Property '{propertyName}' removed as no default value exists.");
            }
        }
        else
        {
            Debug.LogError($"Attempted to clear non-existent property '{propertyName}'.");
        }
    }


    // Helper method to check if a value is the default value for its type
    private bool IsDefaultValue(object value, PropertyDefinition propDef)
    {
        if (keyLoader != null)
        {
            var defaultProp = keyLoader.properties.FirstOrDefault(p => p.PropertyName == propDef.PropertyName);
            if (defaultProp != null)
            {
                return Equals(value, defaultProp.Value);
            }
        }

        if (value == null) return true;

        if (value is bool) return (bool)value == default(bool);
        if (value is int) return (int)value == default(int);
        if (value is string) return string.IsNullOrEmpty((string)value);
        if (value is float) return (float)value == default(float);
        if (value is Vector2) return (Vector2)value == default(Vector2);
        if (value is Vector3) return (Vector3)value == default(Vector3);
        if (value is List<Vector2> vector2List) return vector2List.Count == 0;
        if (value is List<string> stringList) return stringList.Count == 0;
        if (value is List<Vector3> vector3List) return vector3List.Count == 0;
        // Extend for other types as necessary
        return false;
    }

    public void SynchronizePropertiesWithLoader()
    {
        if (keyLoader == null)
        {
            Debug.LogWarning("No key loader assigned for synchronization.");
            return;
        }

        // Create a list of property names currently in the loader
        HashSet<string> loaderPropertyNames = new HashSet<string>(
            keyLoader.properties.Select(p => p.PropertyName)
        );

        // Remove properties that are not present in the loader anymore
        propertyManager.properties.RemoveAll(p => !loaderPropertyNames.Contains(p.PropertyName));

        Debug.Log("Synchronization complete. Properties updated based on the current key loader.");
    }

}

[Serializable]
public class PropertyManager
{

    [SerializeField]                    // A list to hold all property definitions
                                        // associated with a key or loader.
    public List<PropertyDefinition> properties = new List<PropertyDefinition>();
    public SLASHkeyLoader keyLoader;    // Reference to the key loader

    // Assigns a SLASHkeyLoader to this property manager,
    // enabling property initialization and management.
    public void SetKeyLoader(SLASHkeyLoader loader)
    {
        keyLoader = loader;
    }

    // Checks if the current set of properties contains custom data
    // that deviates from the key loader's default properties.
    public bool HasCustomData()
    {
        // Validate keyLoader and its properties list are initialized before proceeding.
        if (keyLoader == null || keyLoader.properties == null)
        {
            Debug.LogWarning("KeyLoader or KeyLoader properties are not initialized.");
            return false;
        }

        // Ensure the properties list is also initialized.
        if (properties == null)
        {
            Debug.LogWarning("Properties list is not initialized.");
            return false;
        }

        // Check each property against the key loader's defaults to identify custom data.
        foreach (var prop in properties)
        {
            var defaultProp = keyLoader.properties.FirstOrDefault(p => p.PropertyName == prop.PropertyName);
            if (defaultProp != null)
            {
                // Check for custom values by comparing each property to its default.
                if (defaultProp.Value == null || prop.Value == null)
                {
                    if (defaultProp.Value != prop.Value)
                    {
                        return true;  // Mismatch found, indicating custom data.
                    }
                }
                else if (!defaultProp.Value.Equals(prop.Value))
                {
                    return true;     // Custom value detected.
                }
            }
            else
            {
                return true;        // Property found that doesn't exist in defaults, indicating custom data.
            }
        }
        return false;               // No custom data found; properties match defaults.
    }

    public void SetProperty<T>(string propertyName, T value)
    {
        // Attempt to find the property by name.
        var property = properties.FirstOrDefault(p => p.PropertyName == propertyName);
        if (property != null)
        {
            // If property exists, update its value directly
            property.Value = value;
        }
        else
        {
            if (keyLoader != null)
                if (keyLoader.allowForeignKeys)
                {
                    // If property doesn't exist, add it as a new one
                    properties.Add(new PropertyDefinition
                    {
                        propertyNameEnum = PropertyNames.Custom,
                        propertyNameCustom = propertyName,
                        Value = value,
                        type = GetTypeFromValue(value)
                    });
                }
        }
    }

    public T GetProperty<T>(string propertyName)
    {
        var property = properties.FirstOrDefault(p => p.PropertyName == propertyName);
        if (property != null)
        {
            try
            {
                // Attempt to convert and return the property value
                return (T)Convert.ChangeType(property.Value, typeof(T));
            }
            catch (InvalidCastException)
            {
                // Handle the case where conversion is not valid
                Debug.LogWarning($"Property '{propertyName}' found, but could not cast to type {typeof(T).Name}.");
            }
        }

        // When the property is not found or a casting error occurs,
        // log all available properties and their types
        Debug.LogWarning($"Property '{propertyName}' not found or mismatched type. Available properties:");
        foreach (var prop in properties)
        {
            Debug.LogWarning($" - {prop.PropertyName}: {prop.Value.GetType().Name} (Value: {prop.Value})");
        }

        return default;
    }

    // Utility to derive PropertyType from value type
    private PropertyType GetTypeFromValue<T>(T value)
    {
        // Implement logic to map from T to PropertyType        
        if (typeof(T) == typeof(bool)) return PropertyType.Bool;
        if (typeof(T) == typeof(int)) return PropertyType.Int;
        if (typeof(T) == typeof(List<Vector2>)) return PropertyType.ListOfVector2;
        if (typeof(T) == typeof(List<Vector3>)) return PropertyType.ListOfVector3;
        if (typeof(T) == typeof(List<Transform>)) return PropertyType.ListOfTransformData;
        if (typeof(T) == typeof(string)) return PropertyType.String;
        if (typeof(T) == typeof(Vector2)) return PropertyType.Vector2;
        if (typeof(T) == typeof(float)) return PropertyType.Float;
        if (typeof(T) == typeof(Vector3)) return PropertyType.Vector3;
        if (typeof(T) == typeof(Transform)) return PropertyType.Transform;
        if (typeof(T) == typeof(Quaternion)) return PropertyType.Quaternion;

        // Add more mappings as necessary

        return PropertyType.Null; // Fallback or unknown type
    }
}

[Serializable]
public class PropertyDefinition
{
    // Predefined property names for quick selection
    // and to reduce errors from typos
    [SerializeField] public PropertyNames propertyNameEnum;

    // Custom property names that can be defined at
    // runtime for additional flexibility
    [SerializeField] public string propertyNameCustom;

    // The data type of the property, which determines
    // how its value is stored and accessed
    [SerializeField] public PropertyType type;

    // Storage for property values of various types,
    // one for each supported type
    public bool boolValue;
    public int intValue;
    public string stringValue;
    public float floatValue;
    // Note: [System.Serializable] attribute may be required for
    // custom types not inherently serializable by Unity
    public List<Vector2> vector2ListValue;
    public List<Vector3> vector3ListValue;
    public List<Transform> transformListValue;
    public DateTime dateTimeValue;
    public Vector2 vector2Value;
    public Vector3 vector3Value;                                   // Additional fields for other supported types can be added here
    public Transform transformValue;
    public Quaternion rotationValue;
    public KLEPNeuron neuronValue;
    //public VoronoiCell voronoiValue;
    // Gets or sets the value of the property in a
    // type-safe manner, converting as necessary
    public object Value
    {
        get
        {
            // Return the value based on the property's type
            switch (type)
            {
                case PropertyType.Bool: return boolValue;
                case PropertyType.Int: return intValue;
                case PropertyType.String: return stringValue;
                case PropertyType.ListOfVector2: return vector2ListValue;
                case PropertyType.ListOfVector3: return vector3ListValue;
                case PropertyType.ListOfTransformData: return transformListValue;
                case PropertyType.DateTime: return dateTimeValue;
                case PropertyType.Vector2: return vector2Value;
                case PropertyType.Float: return floatValue;
                case PropertyType.Vector3: return vector3Value;
                case PropertyType.Transform: return transformValue;
                case PropertyType.Quaternion: return rotationValue;
                case PropertyType.KLEPNeuron: return neuronValue;
               // case PropertyType.VoronoiCell: return voronoiValue;
                default: return null;   // Return null for types that are not handled
            }
        }
        set
        {
            // Set the value based on the property's type, ensuring type safety
            switch (type)
            {
                case PropertyType.Bool:
                    boolValue = Convert.ToBoolean(value);
                    break;
                case PropertyType.Int:
                    intValue = Convert.ToInt32(value);
                    break;
                case PropertyType.String:
                    stringValue = Convert.ToString(value);
                    break;
                case PropertyType.ListOfVector2:
                    vector2ListValue = value as List<Vector2>;
                    break;
                case PropertyType.ListOfVector3:
                    vector3ListValue = value as List<Vector3>;
                    break;
                case PropertyType.ListOfTransformData:
                    transformListValue = value as List<Transform>;
                    break;
                case PropertyType.DateTime:
                    dateTimeValue = Convert.ToDateTime(value);
                    break;
                case PropertyType.Vector2:
                    vector2Value = (Vector2)value;
                    break;
                case PropertyType.Vector3:
                    vector3Value = (Vector3)value;
                    break;
                case PropertyType.Float:
                    floatValue = Convert.ToSingle(value);  // Convert and assign the float value
                    break;
                case PropertyType.Transform:
                    transformValue = (Transform)value;
                    break;
                case PropertyType.Quaternion:
                    rotationValue = (Quaternion)value;
                    break;
                case PropertyType.KLEPNeuron:
                    neuronValue = (KLEPNeuron)value;
                    break;
                case PropertyType.VoronoiCell:
                  //  voronoiValue = (VoronoiCell)value;
                    break;
                    // Additional case handling for other types
            }
        }
    }

    // Combines predefined and custom property names
    // into a single accessible property
    [SerializeField]
    public string PropertyName => propertyNameEnum !=
        PropertyNames.Custom ? propertyNameEnum.ToString() : propertyNameCustom;

    // Provides a fallback default value for a given property type,
    // useful for initializing or resetting properties
    public object GetFallbackDefaultValue(PropertyType type)
    {
        switch (type)
        {
            case PropertyType.Bool: return false;
            case PropertyType.Int: return 0;
            case PropertyType.ListOfVector2: return new List<Vector2Int>();
            case PropertyType.ListOfVector3: return new List<Vector3>();
            case PropertyType.ListOfTransformData: return new List<Transform>();
            case PropertyType.DateTime: return DateTime.MinValue;
            case PropertyType.String: return string.Empty;
            case PropertyType.Vector2: return new Vector2();
            case PropertyType.Float: return 0f;
            case PropertyType.Vector3: return new Vector3();
            case PropertyType.Transform: return null;
            case PropertyType.Quaternion: return Quaternion.identity;
            case PropertyType.KLEPNeuron: return null;
            case PropertyType.VoronoiCell: return null;
            default: return null; // Fallback for unspecified or unknown types
        }
    }

    // Dynamically sets the property value with the specified type,
    // useful for runtime assignments
    public void SetValue<T>(T value)
    {
        this.Value = value;     // Direct assignment, with the actual type conversion handled by the Value property
    }
    public object GetValue()
    {
        switch (type)
        {
            case PropertyType.Bool:
                return boolValue;
            case PropertyType.Int:
                return intValue;
            case PropertyType.String:
                return stringValue;
            case PropertyType.Float:
                return floatValue;
            case PropertyType.Vector2:
                return vector2Value;
            case PropertyType.Vector3:
                return vector3Value;
            case PropertyType.ListOfVector2:
                return vector2ListValue;
            case PropertyType.ListOfVector3:
                return vector3ListValue;
            case PropertyType.ListOfTransformData:
                return transformListValue;
            case PropertyType.DateTime:
                return dateTimeValue;
            case PropertyType.Transform:
                return transformValue;
            case PropertyType.Quaternion:
                return rotationValue;
            case PropertyType.KLEPNeuron:
                return neuronValue;
            case PropertyType.VoronoiCell:
                return null;
           //     return voronoiValue;
            default:
                return null;  // Handle other types or unspecified type
        }
    }
    // Determines and sets the property type based on the type of the value being assigned
    public void DeterminePropertyType<T>()
    {
        // Match the generic type T to a corresponding PropertyType enum value

        if (typeof(T) == typeof(bool)) type = PropertyType.Bool;
        else if (typeof(T) == typeof(int)) type = PropertyType.Int;
        else if (typeof(T) == typeof(string)) type = PropertyType.String;
        else if (typeof(T) == typeof(List<Vector2>)) type = PropertyType.ListOfVector2;
        else if (typeof(T) == typeof(List<Vector3>)) type = PropertyType.ListOfVector3;
        else if (typeof(T) == typeof(List<Transform>)) type = PropertyType.ListOfTransformData;
        else if (typeof(T) == typeof(Vector2)) type = PropertyType.Vector2;
        else if (typeof(T) == typeof(float)) type = PropertyType.Float;
        else if (typeof(T) == typeof(Vector3)) type = PropertyType.Vector3;
        else if (typeof(T) == typeof(Transform)) type = PropertyType.Transform;
        else if (typeof(T) == typeof(Quaternion)) type = PropertyType.Quaternion;
        else if (typeof(T) == typeof(KLEPNeuron)) type = PropertyType.KLEPNeuron;
       // else if (typeof(T) == typeof(VoronoiCell)) type = PropertyType.VoronoiCell;
        // Add more type checks as necessary
        // Use a special null type for unhandled or unknown types

        else type = PropertyType.Null;
    }
}

// Enumerations for PropertyType and PropertyNames provide a structured
// way to handle property types and names,
// reducing errors and improving code clarity and maintainability.
public enum PropertyType
{
    Bool, // the properties we can use
    Int,
    String,
    Float,
    Vector2,
    Vector3,
    List,
    ListOfVector2,
    ListOfVector3,
    ListOfTransformData,
    DateTime,
    Transform,
    Quaternion,
    KLEPNeuron,
    VoronoiCell,
    Null
} // Extend as needed

public enum PropertyNames       // a starter list of common names for strong casting to avoide typos
{
    MakeUnique,
    ImmuneToKeyLifecycle,
    IssuingExecutable,
    KlepExecutableName,
    Positions,
    Transforms,
    IsInUse,
    DateTime,
    Custom
} // Extend as needed

public static class KLEPKeyExtensions
{
    public static SerializableKLEPKey ToSerializable(this KLEPKey key)
    {
        SerializableKLEPKey serializableKey = new SerializableKLEPKey { keyName = key.KeyName };

        foreach (var property in key.propertyManager.properties)
        {
            SerializableProperty serializableProperty = new SerializableProperty
            {
                propertyName = property.PropertyName,
                propertyType = property.type
            };

            switch (property.type)
            {
                case PropertyType.String:
                    serializableProperty.stringValue = property.stringValue;
                    break;
                case PropertyType.Int:
                    serializableProperty.intValue = property.intValue;
                    break;
                case PropertyType.Bool:
                    serializableProperty.boolValue = property.boolValue;
                    break;
                case PropertyType.Float:
                    serializableProperty.floatValue = property.floatValue;
                    break;
                case PropertyType.Vector2:
                    serializableProperty.vector2Value = property.vector2Value;
                    break;
                case PropertyType.Vector3:
                    serializableProperty.vector3Value = property.vector3Value;
                    break;
                case PropertyType.ListOfVector2:
                    serializableProperty.vector2ListValue = property.vector2ListValue;
                    break;
                case PropertyType.ListOfVector3:
                    serializableProperty.vector3ListValue = property.vector3ListValue;
                    break;
                case PropertyType.ListOfTransformData:
                    serializableProperty.transformListValue = new List<SerializableTransform>();
                    foreach (var t in property.transformListValue)
                    {
                        serializableProperty.transformListValue.Add(new SerializableTransform(t));
                    }
                    break;
                    // Add cases for other property types as needed
            }

            serializableKey.properties.Add(serializableProperty);
        }

        return serializableKey;
    }

    public static KLEPKey ToKLEPKey(this SerializableKLEPKey serializableKey, SLASHkeyLoader keyLoader)
    {
        KLEPKey key = ScriptableObject.CreateInstance<KLEPKey>();
        key.KeyName = serializableKey.keyName;
        key.keyLoader = keyLoader;

        foreach (var serializableProperty in serializableKey.properties)
        {
            switch (serializableProperty.propertyType)
            {
                case PropertyType.String:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.stringValue);
                    break;
                case PropertyType.Int:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.intValue);
                    break;
                case PropertyType.Bool:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.boolValue);
                    break;
                case PropertyType.Float:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.floatValue);
                    break;
                case PropertyType.Vector2:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.vector2Value);
                    break;
                case PropertyType.Vector3:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.vector3Value);
                    break;
                case PropertyType.ListOfVector2:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.vector2ListValue);
                    break;
                case PropertyType.ListOfVector3:
                    key.SetProperty(serializableProperty.propertyName, serializableProperty.vector3ListValue);
                    break;
                case PropertyType.ListOfTransformData:
                    List<Transform> transforms = new List<Transform>();
                    foreach (var t in serializableProperty.transformListValue)
                    {
                        GameObject go = new GameObject();
                        t.ApplyToTransform(go.transform);
                        transforms.Add(go.transform);
                    }
                    key.SetProperty(serializableProperty.propertyName, transforms);
                    break;
                    // Add cases for other property types as needed
            }
        }

        return key;
    }
}

// IDEA: Dominate and recessive properties.  In essence this allows you to have properties that arent system critical that when you
// add a property to a dirty loader, then it doesnt add that trait or may or something, lol, im tired.