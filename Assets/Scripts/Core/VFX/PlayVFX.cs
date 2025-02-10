using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayVFX : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    private void OnEnable() {
        if(particleSystems == null) return;
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
    }
}
