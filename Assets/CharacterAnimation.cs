using Mirror;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    const string MOVEMENT = "Movement";
    const string PUNCH = "Punch";
    const string DEATH = "Death";

    Animator animator;
    NetworkAnimator networkAnimator;
    public bool IsAttacking { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    public void Walk(bool move)
    {
        if (!IsAttacking)
        {
            animator.SetBool(MOVEMENT, move);
        }
    }

    public void Punch()
    {
        if (!IsAttacking)
        {
            networkAnimator.SetTrigger(PUNCH);
        }
    }

    public void Death()
    {
        networkAnimator.SetTrigger(DEATH);
    }

    void AttackStart()
    {
        IsAttacking = true;
    }

    void AttackEnd()
    {
        IsAttacking = false;
    }
}
