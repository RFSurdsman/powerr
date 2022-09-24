using Unity.Netcode;
using UnityEngine;

public class FighterController : NetworkBehaviour
{

    private Rigidbody body;

    [SerializeField] float walkSpeed = 3;
    [SerializeField] float jumpSpeed = 1.5f;

    const float rotationY = -90;
    bool isLeft = false;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsOwner)
        {
            RotatePlayer();
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            DetectMovement();
        }
    }

    void DetectMovement()
    {
        body.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * -walkSpeed, body.velocity.y, 0);

        transform.position = new Vector3(transform.position.x, 0, 0);

    }

    void RotatePlayer()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            isLeft = true;
        } 
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            isLeft = false;
        }

        if (isLeft)
        {
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        } else
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Abs(rotationY), 0);
        }
    }
}
