using System;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    private ParticleSystem _ps;
    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    public void Play()
    {
        _ps.Play();
    }
    private void OnParticleSystemStopped()
    {
        Manager.Pool.ReleaseGameObject(this);
    }
}
