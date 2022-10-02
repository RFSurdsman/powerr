using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    private Rigidbody body;
    private CharacterAnimation characterAnimation;

    [SerializeField] float walkSpeed = 3;
    [SerializeField] float jumpSpeed = 1.5f;

    const float rotationY = -90;
    [SyncVar(hook = nameof(OnIsLeftChanged))] bool isLeft = true;
    [SyncVar(hook = nameof(OnHealthChanged))] int health = 100;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        characterAnimation = GetComponentInChildren<CharacterAnimation>();
    }

    void OnIsLeftChanged(bool old, bool newV)
    {
        ResetRotation();
    }

    void OnHealthChanged(int a, int newHealth)
    {
        if (newHealth <= 0)
        {
            characterAnimation.Death();
            CmdRevive();
        }
    }

    void Start()
    {
        if (!hasAuthority)
        {
            gameObject.layer = 7;
        }
    }

    void Update()
    {
        if (hasAuthority && !characterAnimation.IsAttacking && health > 0)
        {
            RotatePlayer();
            AnimatePlayerWalk();
        }
    }

    void FixedUpdate()
    {
        if (hasAuthority && !characterAnimation.IsAttacking && health > 0)
        {
            DetectMovementClient();
        }
    }

    void DetectMovementClient()
    {
        body.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * -walkSpeed, body.velocity.y, 0);

    }

    void RotatePlayer()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            CmdSetIsLeft(true);
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        } 
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            CmdSetIsLeft(false);
            transform.rotation = Quaternion.Euler(0, Mathf.Abs(rotationY), 0);
        }
    }

    void AnimatePlayerWalk()
    {
        var isMoving = Input.GetAxisRaw("Horizontal") != 0;
        characterAnimation.Walk(isMoving);
        if (isMoving)
        {
            transform.position = new Vector3(transform.position.x, 0);
        }
    }

    void ResetRotation()
    {
        if (isLeft)
        {
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Abs(rotationY), 0);
        }
    }

    [Command]
    void CmdSetIsLeft(bool value)
    {
        isLeft = value;
    }

    [Command]
    public void CmdRevive()
    {
        health = 100;
    }


    [Command(requiresAuthority = false)]
    public void ApplyDamageServerRpc(int damage)
    {
        health -= damage;
    }
}
