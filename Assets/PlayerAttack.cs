using Mirror;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour 
{
    CharacterAnimation characterAnimation;

    void Awake()
    {
        characterAnimation = GetComponentInChildren<CharacterAnimation>();
    }

    void Update()
    {
        if (hasAuthority)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            characterAnimation.Punch();
        }    
    }
}
