using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    private Rigidbody body;
    private CharacterAnimation characterAnimation;

    [SerializeField] float walkSpeed = 3;
    [SerializeField] float jumpSpeed = 1.5f;

    const float rotationY = -90;
    readonly NetworkVariable<bool> isLeft = new(writePerm: NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<int> health = new(100);

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        characterAnimation = GetComponentInChildren<CharacterAnimation>();
    }

    void Start()
    {
        isLeft.OnValueChanged += (a, b) => ResetRotation();

        if (IsOwner)
        {
            health.OnValueChanged += (a, newHealth) =>
            {
                if (newHealth <= 0)
                {
                    characterAnimation.Death();
                    ReviveServerRpc();
                }
            };
        }

        if (!IsOwner)
        {
            gameObject.layer = 7;
        }
    }

    void Update()
    {
        if (IsOwner && !characterAnimation.IsAttacking && health.Value > 0)
        {
            RotatePlayer();
            AnimatePlayerWalk();
        }
    }

    void FixedUpdate()
    {
        if (IsOwner && !characterAnimation.IsAttacking && health.Value > 0)
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
            isLeft.Value = true;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        } 
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            isLeft.Value = false;
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
        if (isLeft.Value)
        {
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Abs(rotationY), 0);
        }
    }

    [ServerRpc]
    public void ReviveServerRpc()
    {
        health.Value = 100;
    }


    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageServerRpc(int damage)
    {
        health.Value -= damage;
    }
}
