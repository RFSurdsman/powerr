using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(CharacterAttackController))]
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterMainController : NetworkBehaviour
    {
        CharacterAttackController attack;
        CharacterMovementController movement;

        void Awake()
        {
            attack = GetComponent<CharacterAttackController>();
            movement = GetComponent<CharacterMovementController>();
        }

        public void NormalPunch()
        {
            if (movement.IsGrounded)
            {
                attack.NormalPunch();
            }
        }

        public void MoveRight()
        {
            movement.MoveRight();
        }

        public void MoveLeft()
        {
            movement.MoveLeft();
        }

        public void StopWalk()
        {
            movement.StopWalk();
        }

        public void Jump()
        {
            movement.JumpStart();
        }

        public void Crouch(bool isCrouching)
        {
            movement.Crouch(movement.IsGrounded && isCrouching);
        }
    }
}