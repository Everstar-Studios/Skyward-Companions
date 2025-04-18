using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFile
{
    private List<ISkywardSerializable> serializables = new();
    
    public void Store(ISkywardSerializable serializable)
    {
        serializables.Add(serializable);
    }
    
    public void Save()
    {
        foreach (var serializable in serializables)
        {
            serializable.Serialize();
        }
    }

    public void Load()
    {
        foreach (var serializable in serializables)
        {
            serializable.Deserialize();
        }
    }
}

public interface ISkywardSerializable
{
    void Serialize();
    void Deserialize();
}
