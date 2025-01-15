using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Core;
using UnityEngine;
using Object = UnityEngine.Object;

public class FactoryInstruction
{
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;

    public Action<GameObject> callback;
    
    public FactoryInstruction(GameObject prefab, Vector3 position, Quaternion rotation, Action<GameObject> callback = null)
    {
        this.prefab = prefab;
        this.position = position;
        this.rotation = rotation;
        this.callback = callback;
    }
};

public class GameFactory
{
    private readonly GameContext context;

    public event EventHandler<GameObject> ObjectCreated;
    
    public GameFactory(GameContext context)
    {
        this.context = context;
    }

    public Queue<FactoryInstruction> instructions = new();

    public IEnumerator ProcessQueue()
    {
        while (instructions.Count > 0)
        {
            var instruction = instructions.Dequeue();
            GameObject obj = Object.Instantiate(instruction.prefab, instruction.position, instruction.rotation);
            ObjectCreated?.Invoke(this, obj);
            instruction.callback?.Invoke(obj);
        }

        yield break;
    }

    public void AddInstruction(FactoryInstruction instruction)
    {
        instructions.Enqueue(instruction);
    }
}
