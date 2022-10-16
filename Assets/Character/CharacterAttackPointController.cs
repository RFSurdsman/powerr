using UnityEngine;

namespace Powerr.Character
{
    public class CharacterAttackPointController : MonoBehaviour
    {
        CharacterAttackController characterAttackController;

        [SerializeField] LayerMask collisionLayer;
        [SerializeField] float radius = 1;

        void Awake()
        {
            characterAttackController = GetComponentInParent<CharacterAttackController>();
        }

        void Update()
        {
            DetectCollision();
        }

        void DetectCollision()
        {
            var hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);

            if (hit.Length > 0)
            {
                Debug.Log("HIT " + hit[0].gameObject.name + ", DAMAGE = " + characterAttackController.DamageOnNextAttack);
                gameObject.SetActive(false);
                var characterMovementController = hit[0].GetComponent<CharacterMovementController>();
                characterMovementController.ApplyDamageServerRpc(characterAttackController.DamageOnNextAttack);
            }
        }
    }
}