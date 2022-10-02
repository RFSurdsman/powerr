using Mirror;
using UnityEngine;

public class CharacterAnimationDelegate : NetworkBehaviour
{
    public GameObject leftArmAttackPoint;
    public GameObject rightArmAttackPoint;
    public GameObject leftLegAttackPoint;
    public GameObject rightLegAttackPoint;

    void Awake()
    {
        leftArmAttackPoint.SetActive(false);
        rightArmAttackPoint.SetActive(false);
        leftLegAttackPoint.SetActive(false);
        rightLegAttackPoint.SetActive(false);
    }

    public void LeftArmAttackOn()
    {
        AttackOn(leftArmAttackPoint);
    }

    public void LeftArmAttackOff()
    {
        AttackOff(leftArmAttackPoint);
    }

    public void RightArmAttackOn()
    {
        AttackOn(rightArmAttackPoint);
    }

    public void RightArmAttackOff()
    {
        AttackOff(rightArmAttackPoint);
    }

    public void LeftLegAttackOn()
    {
        AttackOn(leftLegAttackPoint);
    }

    public void LeftLegAttackOff()
    {
        AttackOff(leftLegAttackPoint);
    }

    public void RightLegAttackOn()
    {
        AttackOn(rightLegAttackPoint);
    }

    public void RightLegAttackOff()
    {
        AttackOff(rightLegAttackPoint);
    }

    void AttackOn(GameObject gameObject)
    {
        if (hasAuthority)
        {
            gameObject.SetActive(true);
        }
    }

    void AttackOff(GameObject gameObject)
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
