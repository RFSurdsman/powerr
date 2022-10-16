using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkAnimator))]
    public class CharacterAnimationController : MonoBehaviour
    {
        const int BASE_LAYER = 0;
        const string IDLE_STATE = "Idle";
        const string WALK_STATE = "Walk";
        const string DEATH_STATE = "Death";
        const string MOVEMENT_BOOL_PARAM = "Movement";
        const string NORMAL_PUNCH_TRIGGER_PARAM = "Punch";
        const string DEATH_TRIGGER_PARAM = "Death";

        Animator animator;
        NetworkAnimator networkAnimator;

        public bool IsIdle => IsCurrentStateName(IDLE_STATE);
        public bool IsWalking => IsCurrentStateName(WALK_STATE);
        public bool IsDead => IsCurrentStateName(DEATH_STATE);

        void Awake()
        {
            animator = GetComponent<Animator>();
            networkAnimator = GetComponent<NetworkAnimator>();
        }

        public void Walk(bool isMoving) => animator.SetBool(MOVEMENT_BOOL_PARAM, isMoving);

        public void NormalPunch() => networkAnimator.SetTrigger(NORMAL_PUNCH_TRIGGER_PARAM);

        public void Death() => networkAnimator.SetTrigger(DEATH_TRIGGER_PARAM);

        bool IsCurrentStateName(string name) => animator.GetCurrentAnimatorStateInfo(BASE_LAYER).IsName(name);
    }
}