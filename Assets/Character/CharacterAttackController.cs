using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(CharacterAnimationController))]
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterAttackController : NetworkBehaviour
    {
        CharacterAnimationController characterAnimation;

        public int DamageOnNextAttack
        {
            get => 42;
        }

        void Awake()
        {
            characterAnimation = GetComponent<CharacterAnimationController>();
        }

        public void NormalPunch() => characterAnimation.NormalPunch();
    }
}