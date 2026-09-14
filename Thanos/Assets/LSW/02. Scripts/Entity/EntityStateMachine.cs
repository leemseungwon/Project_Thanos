using System.Collections.Generic;
using LSW._02._Scripts.Common;
using UnityEngine;

namespace LSW._02._Scripts.Entity
{
    public class EntityStateMachine
    {
        public StateData? CurrentState { get; private set; }
        
        private List<StateData> _stateDataList;

        public EntityStateMachine(List<StateData> stateDataList)
        {
            _stateDataList = stateDataList;
        }
        
        public void Initialize(string startState)
        {
            if (!GetState(startState, out StateData startStateData))
            {
                return;
            }
            CurrentState = startStateData;
            CurrentState.Value.State.Enter();
        }

        public void ChangeState(string newState)
        {
            if (CurrentState == null || CurrentState.Value.stateName == newState)
                return;

            CurrentState.Value.State.Exit();

            if (GetState(newState, out StateData newStateData))
            {
                CurrentState = newStateData;
                CurrentState.Value.State.Enter();
            }
            else
            {
                Debug.LogError("State not found: " + newState + "");
            }
        }
        
        private bool GetState(string stateName, out StateData stateData)
        {
            stateData = default;
            if (_stateDataList == null)
                return false;

            StateData foundStateData = _stateDataList.Find(data => data.stateName == stateName);

            stateData = foundStateData;
            return foundStateData.stateName != null;
        }

        public void Update()
        {
            if(CurrentState == null)
                return;
            CurrentState.Value.State.Update();
        }

        public void FixedUpdate()
        {
            if(CurrentState == null)
                return;
            CurrentState.Value.State.FixedUpdate();
        }
        
        ~EntityStateMachine()
        {
            _stateDataList = null;
        }
    }
}