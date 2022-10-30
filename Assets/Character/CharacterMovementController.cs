using Mirror;
using System;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterAnimationController))]
    public class CharacterMovementController : NetworkBehaviour
    {
        enum WalkDirection
        {
            Left,
            Right,
        }

        const float ROTATION_Y = 90;
        const int ENEMY_LAYER = 7;
        const int JUMP_FORCE = 375;
        const int JUMP_AIRBONE_SIDEWAYS_FORCE = 70;
        const float GROUND_COLLISION_SPHERE_RADIUS = 0.1f;

        [SerializeField] LayerMask groundLayer;

        [SyncVar(hook = nameof(OnWalkDirectionChanged))] WalkDirection walkDirection = WalkDirection.Right;
        [SyncVar(hook = nameof(OnHealthChanged))] int health = 100;

        BoxCollider boxCollider;
        CharacterAnimationController characterAnimation;
        Rigidbody characterRigidbody;

        float lastJumpStartTime = 0;
        float lastGroundTime = 0;
        float lastGroundY = 0;

        public bool IsGrounded => Time.fixedTime - lastGroundTime <= Time.fixedDeltaTime;
        public bool IsCanPunch => !characterAnimation.IsJumping;
        bool IsForceUpdateCharacterTransform => characterAnimation.IsIdle || characterAnimation.IsWalking;
        float JumpAirboneSidewaysForceWithScaling => JUMP_AIRBONE_SIDEWAYS_FORCE * Time.fixedDeltaTime * Mathf.Min(1f / (Mathf.Abs(characterRigidbody.velocity.x) + 0.001f), 1f);


        void Awake()
        {
            characterRigidbody = GetComponent<Rigidbody>();
            characterAnimation = GetComponent<CharacterAnimationController>();
            boxCollider = GetComponent<BoxCollider>();
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
            DetectGroundCollision();
        }

        void FixedUpdate()
        {
            EndJumpIfStarted();
        }

        [Client]
        void DetectGroundCollision()
        {
            var hit = Physics.OverlapSphere(transform.position, GROUND_COLLISION_SPHERE_RADIUS, groundLayer);

            if (hit.Length > 0)
            {
                lastGroundTime = Time.time;
                lastGroundY = hit[0].transform.position.y;
            }
        }

        [Client]
        void UpdatePosition()
        {
            if (!IsForceUpdateCharacterTransform || !IsGrounded)
            {
                return;
            }

            transform.position = new Vector3(transform.position.x, lastGroundY);
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
        void StartWalk() => characterAnimation.Walk(true);

        [Client]
        public void StopWalk() => characterAnimation.Walk(false);

        [Client]
        void RotatePlayer(WalkDirection walkDirection)
        {
            if (walkDirection == this.walkDirection || characterAnimation.IsPunching)
            {
                return;
            }

            UpdateRotation(walkDirection);

            if (hasAuthority)
            {
                CmdSetWalkDirection(walkDirection);
            }
        }

        [Client]
        public void MoveRight()
        {
            RotatePlayer(WalkDirection.Right);

            if (characterAnimation.IsJumping)
            {
                characterRigidbody.AddForce(new Vector3(-JumpAirboneSidewaysForceWithScaling, 0));
            }
            else
            {
                StartWalk();
            }
        }

        [Client]
        public void MoveLeft()
        {
            RotatePlayer(WalkDirection.Left);

            if (characterAnimation.IsJumping)
            {
                characterRigidbody.AddForce(new Vector3(JumpAirboneSidewaysForceWithScaling, 0));
            }
            else
            {
                StartWalk();
            }
        }

        [Client]
        public void JumpStart()
        {
            if (!characterAnimation.IsJumping)
            {
                characterAnimation.JumpStart();
                characterRigidbody.AddForce(new Vector3(0, JUMP_FORCE));
                lastJumpStartTime = Time.time;
            }
        }

        [Client]
        void EndJumpIfStarted()
        {
            if (!hasAuthority)
            {
                return;
            }

            var isSufficientlyJumped = Time.time - lastJumpStartTime > Time.fixedDeltaTime;
            if (characterAnimation.IsJumping && IsGrounded && isSufficientlyJumped)
            {
                characterAnimation.JumpEnd();
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
                characterAnimation.Death();
                CmdRevive();
            }
        }

        [Client]
        void OnWalkDirectionChanged(WalkDirection _, WalkDirection newWalkDirection) => UpdateRotation(newWalkDirection);
    }
}