using Assets.Buffs;
using Assets.Model;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    private CharacterBase _owner;
    private List<Buff> _buffs = new List<Buff>();

    public Action<Buff> OnBuffStart;
    public Action<Buff> OnBuffStop;

    public void Init(CharacterBase character)
    {
        _owner = character;
    }

    public void AddBuff(BuffData buffData)
    {
        Buff buff = buffData.CreateBuff();

        buff.Init(this._owner);

        _buffs.Add(buff);

        if(buff.buffIcon != null)
        {
            OnBuffStart?.Invoke(buff);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = _buffs.Count - 1; i >= 0; i--)
        {
            Buff buff = _buffs[i];

            if(!buff.Update(Time.deltaTime))
            {
                RemoveBuff(i);
            }
        }
    }

    public void RemoveBuff(int index)
    {
        Buff buff = _buffs[index];

        buff.Remove();

        _buffs.RemoveAt(index);

        if(buff.buffIcon != null )
        {
            OnBuffStop?.Invoke(buff);
        }
    }
    public void ClearBuff()
    {
        for (int i = _buffs.Count - 1; i >= 0; i--)
        {
            Buff buff = _buffs[i];

            buff.Remove();

            OnBuffStop?.Invoke(buff);
        }

        _buffs.Clear();
    }
}
