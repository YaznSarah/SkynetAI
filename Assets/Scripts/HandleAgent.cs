using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class HandleAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float _handleSpeed = 10f;
    [SerializeField] private Transform opponentTransform;
    private float _totalDistance;
    private float _time;
    private float m_Existential;
    private float _initialDistance;

    public override void Initialize()
    {
        m_Existential = -.002f;
        _initialDistance = Vector3.Distance(transform.position, targetTransform.position);
    }

    public override void OnEpisodeBegin()
    {
        targetTransform.gameObject.GetComponent<ResetObjects>().Reset();
        opponentTransform.gameObject.GetComponent<ResetObjects>().Reset();
        _totalDistance = 0;
        _time = 30f;

        if (gameObject.CompareTag("Opponent"))
        {
            transform.localPosition = new Vector3(-0.0138f, .072f, 6.371f);
        }
        else
            transform.localPosition = new Vector3(-.0138f, .072f, -6.371f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveDirection = actions.DiscreteActions[0];
        LoadActions(moveDirection);
    }

    private void LoadActions(float direction)
    {
        Rigidbody rigidBody = transform.GetComponent<Rigidbody>();
        Vector3 maxSpeed = new Vector3(_handleSpeed, 0, _handleSpeed);
        switch (direction)
        {
            case 0:
                if (Mathf.Abs(rigidBody.velocity.x) < _handleSpeed)
                    rigidBody.AddForce(-60, 0, 0, ForceMode.Acceleration);
                break;
            case 1:
                if (rigidBody.velocity.x < maxSpeed.x)
                    rigidBody.AddForce(60, 0, 0, ForceMode.Acceleration);
                break;
            case 2:
                if (Mathf.Abs(rigidBody.velocity.z) < _handleSpeed)
                    rigidBody.AddForce(0, 0, -60, ForceMode.Acceleration);
                break;
            case 3:
                if (rigidBody.velocity.z < maxSpeed.z)
                    rigidBody.AddForce(0, 0, 60, ForceMode.Acceleration);
                break;
        }
    }


    private void Update()
    {
        float distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        _totalDistance += distance;

        AddReward((_initialDistance / _totalDistance) - 1);

        if (gameObject.CompareTag("Player") && WinTrigger.PlayerScoredLast) // if player scored reward player
        {
            AddReward(10000f);
            Debug.Log("Player scored");
            WinTrigger.ResetBools();
            EndEpisode();
        }

        else if (gameObject.CompareTag("Opponent") && WinTrigger.OpponentScoredLast)
        {
            AddReward(10000f);
            Debug.Log("Opponent scored");
            WinTrigger.ResetBools();
            EndEpisode();
        }

        /*else if (gameObject.CompareTag("Opponent") && WinTrigger.PlayerScoredLast)
                {
                    AddReward(-10000f);
                    Debug.Log("Player scored");
                    WinTrigger.ResetBools();
                    EndEpisode();
                }
                else if (gameObject.CompareTag("Player") && WinTrigger.OpponentScoredLast)
                {
                    AddReward(-10000f);
                    Debug.Log("Opponent scored");
                    WinTrigger.ResetBools();
                    EndEpisode();
                }*/
        Rigidbody targetRigid = targetTransform.gameObject.GetComponent<Rigidbody>();
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else if (_time < 0 && targetRigid.velocity.magnitude < 0.5f)
        {
            EndEpisode();
        }

        AddReward(m_Existential);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[0] = 0;
            }

            if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[0] = 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[0] = 2;
            }

            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[0] = 3;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Puck"))
        {
            AddReward(750f);
            _time = 30f;
        }
        else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("CenterLine"))
        {
            AddReward(-100f);
        }   
    }
}