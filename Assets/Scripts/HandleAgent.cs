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
    [SerializeField] private float _handleSpeed = 14f;
    private float _totalDistance;
    private float _time;
    public override void OnEpisodeBegin()
    {
        targetTransform.gameObject.GetComponent<ResetObjects>().Reset();
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

    private void LoadActions(float direction)
    {
        Rigidbody rigidBody = transform.GetComponent<Rigidbody>();
        Vector3 maxSpeed = new Vector3(14f, 0, 14f);
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

    private void Update()
    {
         float distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        _totalDistance += distance;
        
        Debug.Log(_totalDistance);
        
        SetReward(1 / _totalDistance); 
        
         if (gameObject.CompareTag("Player") && WinTrigger.PlayerScoredLast)
         {
            SetReward(10f);    
         }
         else if (gameObject.CompareTag("Opponent") && WinTrigger.OpponentScoredLast)
         {
             SetReward(10f);
         }
         
        Rigidbody targetRigid = targetTransform.gameObject.GetComponent<Rigidbody>();
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else if (_time < 0 && targetRigid.velocity.magnitude < 0.5f)
        {
            EndEpisode();
        }
        SetReward(-.005f);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Puck"))
        {
            SetReward(2f);
            _time = 30f;
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            SetReward(-2f);
        }
    }
}