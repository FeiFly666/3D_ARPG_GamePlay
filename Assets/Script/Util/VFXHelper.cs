using Assets.Model;
using System;
using UnityEngine;

public static class VFXHelper
{
    //弃用
    public static void Play(Transform owner, GameObject prefabObj, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        if (prefabObj == null) return;

        EffectObject instance = Manager.Pool.GetGameObject<EffectObject>(prefabObj.GetInstanceID());

        if(instance == null)
        {
            Manager.Pool.CreateGameObjectPool<EffectObject>(prefabObj.GetComponent<EffectObject>(), 10, 2);
            instance = Manager.Pool.GetGameObject<EffectObject>(prefabObj.GetInstanceID());
            if(instance == null ) { return; }
        }
        //instance.transform.SetParent(owner.transform, false);

        instance.transform.position = position + owner.transform.position;
        instance.transform.eulerAngles = rotation;
        instance.transform.localScale = scale;


        instance.Play();
    }

    public static void Play(Transform owner, Transform player ,EffectData effect)
    {
        GameObject prefabObj = effect.VFXPrefab;

        if (prefabObj == null) return;

        Debug.Log($"current VFX : {prefabObj.name}");

        EffectObject instance = Manager.Pool.GetGameObject<EffectObject>(prefabObj.GetInstanceID());

        if (instance == null)
        {
            Manager.Pool.CreateGameObjectPool<EffectObject>(prefabObj.GetComponent<EffectObject>(), 10, 2);
            instance = Manager.Pool.GetGameObject<EffectObject>(prefabObj.GetInstanceID());
            if (instance == null) { return; }
        }
        instance.transform.SetParent(owner, false);

        instance.transform.localPosition = effect.position;
        //instance.transform.eulerAngles = effect.rotation;

        //instance.transform.rotation = owner.rotation * Quaternion.Euler(effect.rotation);

        instance.transform.SetParent(null);

        instance.transform.rotation = owner.rotation * Quaternion.Euler(effect.rotation);
        instance.transform.localScale = effect.scale;

        instance.Play();
    }
}
