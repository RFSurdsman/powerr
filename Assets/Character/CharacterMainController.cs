using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(CharacterAttackController))]
    [RequireComponent(typeof(CharacterMovementController))]
    public class CharacterMainController : NetworkBehaviour
    {
        public CharacterAttackController Attack { get; private set; }
        public CharacterMovementController Movement { get; private set; }

        void Awake()
        {
            Attack = GetComponent<CharacterAttackController>();
            Movement = GetComponent<CharacterMovementController>();
        }
    }
}