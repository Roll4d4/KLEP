using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SLASHsavekey : MonoBehaviour
{
    public enum SaveFormat { XML, Binary }
    public SaveFormat saveFormat = SaveFormat.XML;

    public static SLASHsavekey Instance { get; private set; }
    private string saveDirectory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this manager persistent across scenes
            saveDirectory = Path.Combine(Application.persistentDataPath, "KLEPKeys");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveKey(string keyName, KLEPKey key)
    {
        SerializableKLEPKey serializableKey = key.ToSerializable();
        string filePath = Path.Combine(saveDirectory, keyName + (saveFormat == SaveFormat.XML ? ".xml" : ".bin"));

        switch (saveFormat)
        {
            case SaveFormat.XML:
                SaveToXML(serializableKey, filePath);
                break;
            case SaveFormat.Binary:
                SaveToBinary(serializableKey, filePath);
                break;
        }

        Debug.Log("Key saved to " + filePath);
    }

    public KLEPKey LoadKey(string keyName, SLASHkeyLoader keyLoader)
    {
        string filePath = Path.Combine(saveDirectory, keyName + (saveFormat == SaveFormat.XML ? ".xml" : ".bin"));
        if (!File.Exists(filePath))
        {
            Debug.LogError("Save file not found for key: " + keyName);
            return null;
        }

        SerializableKLEPKey serializableKey = null;
        switch (saveFormat)
        {
            case SaveFormat.XML:
                serializableKey = LoadFromXML(filePath);
                break;
            case SaveFormat.Binary:
                serializableKey = LoadFromBinary(filePath);
                break;
        }

        if (serializableKey != null)
        {
            return serializableKey.ToKLEPKey(keyLoader);
        }

        return null;
    }

    public bool IsSaveFileExists(string keyName)
    {
        string filePath = Path.Combine(saveDirectory, keyName + (saveFormat == SaveFormat.XML ? ".xml" : ".bin"));
        return File.Exists(filePath);
    }

    private void SaveToXML(SerializableKLEPKey keyData, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SerializableKLEPKey));
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, keyData);
        }
    }

    private SerializableKLEPKey LoadFromXML(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SerializableKLEPKey));
        using (StreamReader reader = new StreamReader(filePath))
        {
            return (SerializableKLEPKey)serializer.Deserialize(reader);
        }
    }

    private void SaveToBinary(SerializableKLEPKey keyData, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write(JsonUtility.ToJson(keyData));
        }
    }

    private SerializableKLEPKey LoadFromBinary(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            BinaryReader reader = new BinaryReader(fs);
            return JsonUtility.FromJson<SerializableKLEPKey>(reader.ReadString());
        }
    }
}


[Serializable]
public class SerializableProperty
{
    public string propertyName;
    public PropertyType propertyType;
    public string stringValue;
    public int intValue;
    public bool boolValue;
    public float floatValue;
    public Vector2 vector2Value;
    public Vector3 vector3Value;
    public List<Vector2> vector2ListValue;
    public List<Vector3> vector3ListValue;
    public List<SerializableTransform> transformListValue;

    public SerializableProperty() { } // Parameterless constructor for XML serialization
}

[Serializable]
public class SerializableTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public SerializableTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
    }

    public void ApplyToTransform(Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = localScale;
    }

    public SerializableTransform() { }
}

[Serializable]
public class SerializableKLEPKey
{
    public string keyName;
    public List<SerializableProperty> properties = new List<SerializableProperty>();

    public SerializableKLEPKey() { }
}