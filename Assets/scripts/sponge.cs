using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sponge : MonoBehaviour
{
    private float damp = 0.9f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x * damp, rb.linearVelocity.y, rb.linearVelocity.z * damp);
    }
}
