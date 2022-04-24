using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DefendGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-0.0138f, .072f, 6.371f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    private void LoadActions(float direction)
    {
        Rigidbody rigidBody = transform.GetComponent<Rigidbody>();
        OpponentInput opponentInput = GetComponent<OpponentInput>();
    
        Vector3 maxSpeed = new Vector3(opponentInput.HandleSpeed, 0, opponentInput.HandleSpeed);
        switch (direction)
        {
            case 0:
                if (Mathf.Abs(rigidBody.velocity.x) < opponentInput.HandleSpeed)
                    rigidBody.AddForce(-60, 0, 0, ForceMode.Acceleration);
                break;
            case 1:
                if (rigidBody.velocity.x < maxSpeed.x)
                    rigidBody.AddForce(60, 0, 0, ForceMode.Acceleration);
                break;
            case 2:
                if (Mathf.Abs(rigidBody.velocity.z) < maxSpeed.z)
                    rigidBody.AddForce(0, 0, -60, ForceMode.Acceleration);
                break;
            case 3:
                if (rigidBody.velocity.z < maxSpeed.z)
                    rigidBody.AddForce(0, 0, 60, ForceMode.Acceleration);
                break;
        }       

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveDirection = actions.DiscreteActions[0];
        LoadActions(moveDirection);

        /*
        rigidBody.AddForce(moveSpeed * moveUp, 0, 0, ForceMode.Acceleration);
        rigidBody.AddForce(moveSpeed * moveDown, 0, 0, ForceMode.Acceleration);
        rigidBody.AddForce(0, 0, moveSpeed * moveDown, ForceMode.Acceleration);*/
    }

    /*
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Puck"))
        {
            SetReward(1f);
            EndEpisode();
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}