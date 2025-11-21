using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sponge : MonoBehaviour
{
    [Tooltip("Minimum horizontal speed a character must have to move the sponge")]
    public float pushVelocityThreshold = 0.5f;
    [Tooltip("How quickly the sponge comes to rest (units/sec^2)")]
    public float idleDeceleration = 60f;

    private Rigidbody rb;
    private bool pushedThisFrame;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (!pushedThisFrame)
        {
            // No valid push this frame, kill horizontal velocity but keep gravity
            Vector3 velocity = rb.linearVelocity;
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, idleDeceleration * Time.fixedDeltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0f, idleDeceleration * Time.fixedDeltaTime);
            rb.linearVelocity = velocity;
        }

        pushedThisFrame = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (rb == null) return;

        Rigidbody otherRb = collision.rigidbody;
        if (otherRb == null) return;

        if (!IsCharacter(collision.transform)) return;

        Vector3 horizontalVelocity = otherRb.linearVelocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < pushVelocityThreshold * pushVelocityThreshold)
        {
            return;
        }

        Vector3 pushVelocity;
        if (!TryGetPushVelocity(collision, horizontalVelocity, out pushVelocity))
        {
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.x = pushVelocity.x;
        velocity.z = pushVelocity.z;
        rb.linearVelocity = velocity;
        pushedThisFrame = true;
    }

    private bool IsCharacter(Transform other)
    {
        if (other == null) return false;
        if (other.GetComponent<corgi_move>() != null) return true;
        return other.GetComponentInParent<corgi_move>() != null;
    }

    private bool TryGetPushVelocity(Collision collision, Vector3 pusherVelocity, out Vector3 pushVelocity)
    {
        pushVelocity = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            Vector3 normal = contact.normal;
            Vector3 horizontalNormal = new Vector3(normal.x, 0f, normal.z);
            if (horizontalNormal.sqrMagnitude < 0.0001f)
            {
                continue;
            }

            horizontalNormal.Normalize();
            float pushComponent = Vector3.Dot(pusherVelocity, horizontalNormal * -1f);
            if (pushComponent <= pushVelocityThreshold)
            {
                continue;
            }

            // Transfer the full horizontal velocity, not just the component along the normal
            pushVelocity = pusherVelocity;
            return true;
        }

        return false;
    }
}
