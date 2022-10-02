using Unity.Netcode;
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
        if (IsOwner)
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
