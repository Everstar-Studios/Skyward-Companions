using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFile
{
    private Dictionary<string, ISkywardSerializable> serializables = new();
    
    public void Store(string key, ISkywardSerializable serializable)
    {
        serializables[key] = serializable;
    }
    
    public ISkywardSerializable Retrieve(string key)
    {
        return serializables[key];
    }
    
    public void Save(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(serializables.Count);
        foreach (var (key, serializable) in serializables)
        {
            writer.Write(key);
            serializable.Serialize(writer);
        }
        stream.Close();
    }

    public void Load(string path)
    {
        if (!File.Exists(path)) 
            return;
        
        FileStream stream = new FileStream(path, FileMode.Open);
        BinaryReader reader = new BinaryReader(stream);
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            string key = reader.ReadString();
            if (serializables.TryGetValue(key, out var serializable))
            {
                serializable.Deserialize(reader);
            }
        }

        stream.Close();
    }
}

public interface ISkywardSerializable
{
    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader);
}
