using Assets.StateMachine.CommonState;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.StateMachine
{
    public class StateMachine<T> where T : CharacterBase
    {
        protected T owner;

        private IState<T> CurrentState;

        private Dictionary<Type, IState<T>> _stateCache = new Dictionary<Type, IState<T>>();

        public StateMachine(T owner)
        {
            this.owner = owner;
        }
        public void Initialize<S>() where S : class, IState<T>, new()
        {
            ChangeState<S>();
        }

        public void ChangeState<S>() where S : class, IState<T>, new()
        {
            Type type = typeof(S);
            if (!_stateCache.TryGetValue(type, out IState<T> nextState))
            {
                nextState = Manager.Pool.GetClass<S>();
                nextState.Init(owner, this);
                _stateCache.Add(type, nextState);
            }

            if (CurrentState == nextState)
            {
                if(!(CurrentState is CharacterHitState<T> || CurrentState is CharacterStaggerState<T>))
                    return;
            }

            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState?.Enter();
        }

        public void Update() => CurrentState?.Update();
    }
}
