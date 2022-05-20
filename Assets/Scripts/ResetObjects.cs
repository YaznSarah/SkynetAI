using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjects : MonoBehaviour
{
    private Vector3 _initPosition;
    private Vector3 _initVelocity;
    private Quaternion _initRotation;
    private Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        _initPosition = _rigidBody.position;
        _initRotation = _rigidBody.rotation;
        _initVelocity = _rigidBody.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Reset();
            WinTrigger.ScoreP1 = 0;
            WinTrigger.ScoreP2 = 0;
        }
    }

    public void Reset()
    {
        transform.position = _initPosition;
        transform.rotation = _initRotation;
        _rigidBody.velocity = _initVelocity;
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
        if ((other.gameObject.CompareTag("P1_Win") || other.gameObject.CompareTag("P2_Win")) && gameObject.CompareTag("Puck"))
        {
            Reset();
        }
    }
}