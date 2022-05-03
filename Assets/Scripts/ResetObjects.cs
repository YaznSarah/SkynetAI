using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjects : MonoBehaviour
{
    Vector3 initPosition;
    Vector3 initVelocity;
    Quaternion initRotation;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        initPosition = rigidBody.position;
        initRotation = rigidBody.rotation;
        initVelocity = rigidBody.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Reset();
        }
    }

    public void Reset()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
        rigidBody.velocity = initVelocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        /*if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent"))
        {
            Reset();
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("P1_Win") || other.gameObject.CompareTag("P2_Win"))
        {
            Reset();
        }
    }
}