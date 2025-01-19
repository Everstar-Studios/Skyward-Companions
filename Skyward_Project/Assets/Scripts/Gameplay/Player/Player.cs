using System;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [HideInInspector]
    public InputComponent inputComponent;

    private void Awake()
    {
        inputComponent = GetComponent<InputComponent>();
    }

    void Start()
    {
        SetupMovement();
    }
}
