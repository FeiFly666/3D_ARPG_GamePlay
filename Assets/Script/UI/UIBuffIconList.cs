using Assets.Buffs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuffIconList : MonoBehaviour
{
    [SerializeField] public UIBuffIcon iconPrefab;
    private Dictionary<Buff, UIBuffIcon> _Buff2Icon = new Dictionary<Buff, UIBuffIcon>();
    private PlayerController player;
    private void Start()
    {
        player = Manager.Character.player;
        if(player != null)
        {
            player.buffCtrl.OnBuffStart += SetNewBuff;
            player.buffCtrl.OnBuffStop += RemoveBuff;
        }
    }
    private void Update()
    {
        if(player == null)
        {
            player = Manager.Character.player;
            if (player != null)
            {
                player.buffCtrl.OnBuffStart += SetNewBuff;
                player.buffCtrl.OnBuffStop += RemoveBuff;
            }
        }
    }
    private void SetNewBuff(Buff buff)
    {
        if(buff == null) return;

        if(_Buff2Icon.ContainsKey(buff))
        {
            return;
        }
        else
        {
            CreateIcon(buff);
        }
    }
    private void CreateIcon(Buff buff)
    {
        UIBuffIcon icon = Manager.Pool.GetGameObject<UIBuffIcon>(iconPrefab.gameObject.GetInstanceID());

        if(icon == null)
        {
            Manager.Pool.CreateGameObjectPool<UIBuffIcon>(iconPrefab, 10, 5);

            icon = Manager.Pool.GetGameObject<UIBuffIcon>(iconPrefab.gameObject.GetInstanceID());
        }

        if(icon != null)
        {
            icon.gameObject.transform.SetParent(this.transform, false);

            icon.InitIcon(buff.buffIcon);

            _Buff2Icon.Add(buff, icon);
        }
    }
    private void RemoveBuff(Buff buff)
    {
        UIBuffIcon theIcon = _Buff2Icon[buff];

        if(theIcon != null)
        {
            theIcon.StopShow();
        }

        _Buff2Icon.Remove(buff);

        //Manager.Pool.ReleaseGameObject<UIBuffIcon>(theIcon);
    }
    private void OnDestroy()
    {
        if (player != null)
        {
            player.buffCtrl.OnBuffStart -= SetNewBuff;
            player.buffCtrl.OnBuffStop -= RemoveBuff;
        }
    }
}