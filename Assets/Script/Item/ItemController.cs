using Assets.Model;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemController : MonoBehaviour
{
    private CharacterBase _owner;

    public ItemData currentItem;

    private Dictionary<ItemData, float>_ItemColdDown = new Dictionary<ItemData, float>();

    public Action<float, float> OnItemProgressChanged;
    public Action<ItemData> OnItemChanged;


    public void Init(CharacterBase owner, ItemData newItem)
    {
        _owner = owner;

        ChangeItem(newItem);
    }

    private void Update()
    {
        if(currentItem != null && _ItemColdDown.ContainsKey(currentItem) && Time.time - _ItemColdDown[currentItem] <= currentItem.coldDown)
        {
            OnItemProgressChanged?.Invoke(currentItem.coldDown, _ItemColdDown[currentItem]);
        }
    }

    public void ChangeItem(ItemData item)
    {
        if(item == null) return;

        currentItem = item;

        OnItemChanged?.Invoke(currentItem);

        if(!_ItemColdDown.ContainsKey(currentItem))
        {
            _ItemColdDown.Add(currentItem, -10000);
        }

        OnItemProgressChanged?.Invoke(currentItem.coldDown, _ItemColdDown[currentItem]);
    }

    public void UseItem()
    {
        if(currentItem == null) return;

        if(currentItem.Buff == null) return;
        //currentItem.Use();

        if(!_ItemColdDown.ContainsKey(currentItem) || Time.time - _ItemColdDown[currentItem] > currentItem.coldDown)
        {
            _owner.buffCtrl.AddBuff(currentItem.Buff);


            if (_ItemColdDown.ContainsKey(currentItem))
            {
                _ItemColdDown[currentItem] = Time.time;
            }
            else
            {
                _ItemColdDown.Add(currentItem, Time.time);
            }
        }
    }
}
