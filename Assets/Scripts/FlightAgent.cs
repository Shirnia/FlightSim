using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;


public class FlightAgent : Agent
{
    private PlayerController playerController;
    private LoadConfig config;
    // Start is called before the first frame update

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        config = new LoadConfig();
    }

     public override void OnEpisodeBegin()
    {
    }
    
     public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(float[] vectorAction)
    {   
        float pitch;
        float roll;
        foreach (BranchData bd in config.ActionInfo.branches)
        {
            float value;
            switch (bd.action_type){
                case "Throttle":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "Pitch":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "Roll":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "Yaw":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "Flaps":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "FireMissile":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
                case "FireCannon":
                    value = vectorAction[0];
                    playerController.SetThrottleInput(value);
                    break;
            }
        }
        // playerController.OnRollPitchInput();
        // playerController.OnYawInput();
        // playerController.OnFlapsInput();
        // playerController.OnFireMissile();
        // playerController.OnFireCannon();

    }

    public override void Heuristic(float[] actionsOut)
    {   

    }

}
