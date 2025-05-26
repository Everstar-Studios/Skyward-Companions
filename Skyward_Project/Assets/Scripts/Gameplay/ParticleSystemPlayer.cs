using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour
{
    private List<ParticleSystem> particles = new();

    private void Awake()
    {
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            particles.Add(particle);
    }

    public void PlayParticles()
    {
        particles.ForEach(p => p.Play());
    }
}
