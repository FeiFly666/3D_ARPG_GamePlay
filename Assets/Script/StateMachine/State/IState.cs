using Assets.StateMachine;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T> where T : CharacterBase
{
    public void Init(T owner, StateMachine<T> machine);
    public void Enter();
    public void Update();
    public void Exit();
}
