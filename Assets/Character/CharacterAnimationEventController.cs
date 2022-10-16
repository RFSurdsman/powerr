using Mirror;
using UnityEngine;

namespace Powerr.Character
{
    public class CharacterAnimationEventController : NetworkBehaviour
    {
        [SerializeField] GameObject leftArmAttackPoint;
        [SerializeField] GameObject rightArmAttackPoint;
        [SerializeField] GameObject leftLegAttackPoint;
        [SerializeField] GameObject rightLegAttackPoint;

        void Awake()
        {
            leftArmAttackPoint.SetActive(false);
            rightArmAttackPoint.SetActive(false);
            leftLegAttackPoint.SetActive(false);
            rightLegAttackPoint.SetActive(false);
        }

#pragma warning disable IDE0051

        /*
         * The functions here are called by the keyframe events on characters, they are not
         * referenced by the code.
         */

        void LeftArmAttackOn() => AttackOn(leftArmAttackPoint);
        void LeftArmAttackOff() => AttackOff(leftArmAttackPoint);
        void RightArmAttackOn() => AttackOn(rightArmAttackPoint);
        void RightArmAttackOff() => AttackOff(rightArmAttackPoint);
        void LeftLegAttackOn() => AttackOn(leftLegAttackPoint);
        void LeftLegAttackOff() => AttackOff(leftLegAttackPoint);
        void RightLegAttackOn() => AttackOn(rightLegAttackPoint);
        void RightLegAttackOff() => AttackOff(rightLegAttackPoint);

#pragma warning restore IDE0051

        void AttackOn(GameObject gameObject)
        {
            if (hasAuthority)
            {
                gameObject.SetActive(true);
            }
        }

        void AttackOff(GameObject gameObject)
        {
            if (hasAuthority && gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
    }
}