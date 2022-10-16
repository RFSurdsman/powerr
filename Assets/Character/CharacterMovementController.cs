using Mirror;
using System;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(CharacterAnimationController))]
    public class CharacterMovementController : NetworkBehaviour
    {
        public enum WalkDirection
        {
            Left,
            Right,
        }

        const float ROTATION_Y = 90;
        const int ENEMY_LAYER = 7;

        CharacterAnimationController characterAnimationController;

        [SyncVar(hook = nameof(OnWalkDirectionChanged))] WalkDirection walkDirection = WalkDirection.Right;
        [SyncVar(hook = nameof(OnHealthChanged))] int health = 100;

        bool IsForceUpdateCharacterTransform => characterAnimationController.IsIdle || characterAnimationController.IsWalking;

        void Awake()
        {
            characterAnimationController = GetComponent<CharacterAnimationController>();
        }

        void Start()
        {
            if (!hasAuthority)
            {
                gameObject.layer = ENEMY_LAYER;
            }
        }

        void Update()
        {
            UpdatePosition();
            UpdateRotation();
        }

        [Client]
        void UpdatePosition()
        {
            if (!IsForceUpdateCharacterTransform)
            {
                return;
            }

            transform.position = new Vector3(transform.position.x, 0);
        }

        [Client]
        void UpdateRotation()
        {
            if (!IsForceUpdateCharacterTransform)
            {
                return;
            }

            UpdateRotation(walkDirection);
        }

        [Client]
        void UpdateRotation(WalkDirection walkDirection)
        {
            transform.rotation = walkDirection switch
            {
                WalkDirection.Left => Quaternion.Euler(0, ROTATION_Y, 0),
                WalkDirection.Right => Quaternion.Euler(0, -ROTATION_Y, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(WalkDirection)),
            };
        }

        [Client]
        public void StartWalk() => characterAnimationController.Walk(true);

        [Client]
        public void StopWalk() => characterAnimationController.Walk(false);

        [Client]
        public void RotatePlayer(WalkDirection walkDirection)
        {
            if (walkDirection == this.walkDirection || !characterAnimationController.IsIdle && !characterAnimationController.IsWalking)
            {
                return;
            }

            UpdateRotation(walkDirection);

            if (hasAuthority)
            {
                CmdSetWalkDirection(walkDirection);
            }
        }

        [Command]
        void CmdSetWalkDirection(WalkDirection walkDirection) => this.walkDirection = walkDirection;

        [Command]
        public void CmdRevive() => health = 100;

        [Command(requiresAuthority = false)]
        public void ApplyDamageServerRpc(int damage) => health -= damage;

        [Client]
        void OnHealthChanged(int _, int newHealth)
        {
            if (hasAuthority && newHealth <= 0)
            {
                characterAnimationController.Death();
                CmdRevive();
            }
        }

        [Client]
        void OnWalkDirectionChanged(WalkDirection _, WalkDirection newWalkDirection) => UpdateRotation(newWalkDirection);
    }
}