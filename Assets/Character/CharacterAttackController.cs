using UnityEngine;

namespace Powerr.Character
{
    [RequireComponent(typeof(CharacterAnimationController))]
    public class CharacterAttackController : MonoBehaviour
    {
        CharacterAnimationController characterAnimationController;

        public int DamageOnNextAttack
        {
            get => 42;
        }

        void Awake()
        {
            characterAnimationController = GetComponent<CharacterAnimationController>();
        }

        public void NormalPunch() => characterAnimationController.NormalPunch();
    }
}