using System;
using UnityEngine;

public class Ball2 : MonoBehaviour
{
    Rigidbody rb;
    private Vector3 lastVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lastVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        var speed = lastVelocity.magnitude;
        var direction = Vector3.Reflect(lastVelocity.normalized, other.contacts[0].normal);

        rb.linearVelocity = direction * Mathf.Max(speed, 0);
    }
}