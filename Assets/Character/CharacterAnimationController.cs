using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkAnimator))]
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterAnimationController : NetworkBehaviour
    {
        const int BASE_LAYER = 0;
        const string IDLE_STATE = "Idle";
        const string WALK_STATE = "Walk";
        const string PUNCH_STATE = "Punch";
        const string DEATH_STATE = "Death";
        const string JUMP_STATE = "Jump";
        const string MOVEMENT_BOOL_PARAM = "Movement";
        const string NORMAL_PUNCH_TRIGGER_PARAM = "Punch";
        const string DEATH_TRIGGER_PARAM = "Death";
        const string JUMP_START_TRIGGER_PARAM = "Jump Start";
        const string JUMP_END_TRIGGER_PARAM = "Jump End";

        Animator animator;
        NetworkAnimator networkAnimator;
        CharacterMovementController movement;
        bool isJumping = false;

        [SyncVar(hook = nameof(OnRootMotionChanged))] bool applyRootMotion = true;

        public bool IsIdle => !IsJumping && IsCurrentStateName(IDLE_STATE);
        public bool IsWalking => !IsJumping && IsCurrentStateName(WALK_STATE);
        public bool IsPunching => !IsJumping && IsCurrentStateName(PUNCH_STATE);
        public bool IsDead => !IsJumping && IsCurrentStateName(DEATH_STATE);
        public bool IsJumping => isJumping || IsCurrentStateName(JUMP_STATE);

        bool IsCurrentStateName(string name) => animator.GetCurrentAnimatorStateInfo(BASE_LAYER).IsName(name);

        void Awake()
        {
            animator = GetComponent<Animator>();
            networkAnimator = GetComponent<NetworkAnimator>();
            movement = GetComponent<CharacterMovementController>();
        }

        void FixedUpdate()
        {
            UpdateRootMotion();
        }

        [Client]
        public void Walk(bool isMoving) => animator.SetBool(MOVEMENT_BOOL_PARAM, isMoving);

        [Client]
        public void NormalPunch() => networkAnimator.SetTrigger(NORMAL_PUNCH_TRIGGER_PARAM);

        [Client]
        public void Death() => networkAnimator.SetTrigger(DEATH_TRIGGER_PARAM);

        [Client]
        public void JumpStart()
        {
            isJumping = true;
            networkAnimator.SetTrigger(JUMP_START_TRIGGER_PARAM);
        }

        [Client]
        public void JumpEnd()
        {
            isJumping = false;
            networkAnimator.SetTrigger(JUMP_END_TRIGGER_PARAM);
        }

        [Client]
        void UpdateRootMotion()
        {
            if (!hasAuthority)
            {
                return;
            }

            UpdateRootMotion(movement.IsGrounded);
            CmdUpdateRootMotion(movement.IsGrounded);
        }

        [Client]
        void UpdateRootMotion(bool applyRootMotion)
        {
            if (IsIdle || IsWalking)
            {
                animator.applyRootMotion = applyRootMotion;
            }
            else if (IsJumping)
            {
                animator.applyRootMotion = false;
            }
            else
            {
                animator.applyRootMotion = true;
            }
        }

        [Command]
        void CmdUpdateRootMotion(bool applyRootMotion)
        {
            this.applyRootMotion = applyRootMotion;
        }

        [Client]
        void OnRootMotionChanged(bool _, bool applyRootMotion) => UpdateRootMotion(applyRootMotion);
    }
}