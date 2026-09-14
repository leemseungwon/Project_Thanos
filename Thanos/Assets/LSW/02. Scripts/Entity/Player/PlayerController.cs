
using System.Collections.Generic;
using LSW._02._Scripts.Common;
using LSW._02._Scripts.Entity.Player.Component;
using LSW._02._Scripts.Entity.Player.States;
using LSW._03._So.Inputs;
using UnityEngine;

namespace LSW._02._Scripts.Entity.Player
{
    public class PlayerController : BaseEntity
    {
        [Header("Input")]
        [field: SerializeField] public InputData Input { get; private set; }
        
        [Header("Camera")]
        [field:SerializeField] public Camera PlayerCamera { get; private set; }

        public PlayerMovementComponent Movement { get; private set; }
        public PlayerCombatComponent Combat { get; private set; }
        public PlayerStoneComponent Stone { get; private set; }
        
        public Vector2 LookDirection { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            if (GetCompo(out PlayerMovementComponent movement))
                Movement = movement;

            if (GetCompo(out PlayerCombatComponent combat))
                Combat = combat;
            
            if (GetCompo(out PlayerStoneComponent stone))
                Stone = stone;

            InitStates();

            if (Input != null)
            {
                Input.OnSwitchNextPressed += HandleStoneSwitch;
                Input.OnAttackPressed += HandleAttack;
                Input.OnDashPressed += HandleDash;
            }
        }

        private void InitStates()
        {
            StateMachine = new EntityStateMachine
            (
                new List<StateData>{
                    new StateData
                    {
                        stateName = "PlayerIdleState",
                        State = new PlayerIdleState(this)
                    },
                    new StateData
                    {
                        stateName = "PlayerMoveState",
                        State = new PlayerMoveState(this)
                    },
                    new StateData
                    {
                        stateName = "PlayerAttackState",
                        State = new PlayerAttackState(this)
                    },
                    new StateData
                    {
                        stateName = "PlayerDashState",
                        State = new PlayerDashState(this)
                    }
                }
            );
        }

        private void Start()
        {
            StateMachine.Initialize("PlayerIdleState");
        }

        protected override void Update()
        {
            base.Update();
            LookAtMouse();
        }

        private void LookAtMouse()
        {
            Vector3 mouseWorldPosition =
                PlayerCamera.ScreenToWorldPoint(
                    new Vector3(Input.MousePosition.x, Input.MousePosition.y,
                        Mathf.Abs(PlayerCamera.transform.position.z - transform.position.z)));
            Vector2 direction = mouseWorldPosition - transform.position;

            if (direction.sqrMagnitude <= 0.001f)
                return;
            
            LookDirection = direction.normalized;

            float angle = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        

        private void HandleStoneSwitch(bool isNext)
        {
            if (isNext)
            {
                Stone.SwitchNext();
            }
            else
            {
                Stone.SwitchPrevious();
            }
        }

        private void HandleDash()
        {
            if(StateMachine == null)
                return;
            StateMachine.ChangeState("PlayerDashState");
        }
        
        private void HandleAttack()
        {
            if(StateMachine == null)
                return;
            StateMachine.ChangeState("PlayerAttackState");
        }

        private void OnDestroy()
        {
            if (Input != null)
            {
                Input.OnSwitchNextPressed -= HandleStoneSwitch;
                Input.OnAttackPressed -= HandleAttack;
                Input.OnDashPressed -= HandleDash;
            }
        }
    }
}