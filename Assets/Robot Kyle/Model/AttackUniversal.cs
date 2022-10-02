using UnityEngine;

public class AttackUniversal : MonoBehaviour
{
    public LayerMask collisionLayer;
    public float radius = 1f;
    public float damage = 2f;

    void Update()
    {
        DetectCollision();
    }

    void DetectCollision()
    {
        var hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);

        if (hit.Length > 0)
        {
            print("We hit the " + hit[0].gameObject.name);

            gameObject.SetActive(false);

            hit[0].GetComponent<PlayerMovement>().ApplyDamageServerRpc(42);
        }
    }
}
