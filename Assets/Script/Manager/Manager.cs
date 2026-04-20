using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static PoolManager _pool;

    private static CharacterManager _character;

    private static EventManager _event;

    public static PoolManager Pool
    {
        get { return _pool; }
    }
    public static CharacterManager Character
    { get { return _character; } }

    public static EventManager Event
    {
        get { return _event; }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        InitManager();
    }

    private void InitManager()
    {
        if( _pool == null )
             _pool = this.AddComponent<PoolManager>();
        if( _character == null )
            _character = this.AddComponent<CharacterManager>();
        if(_event == null )
            _event = this.AddComponent<EventManager>();
    }
}
