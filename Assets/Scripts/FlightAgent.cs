using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem; 
using System.Linq;


public class FlightAgent : Agent
{
    private PlayerController playerController;
    private AIController aiController;
    private LoadConfig config;
    private float t;
    // Start is called before the first frame update

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        aiController = GetComponent<AIController>();
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
        float throttle = 0;
        float pitch = 0;
        float roll = 0;
        float yaw = 0;
        int index = 0;
        foreach (BranchData bd in config.ActionInfo.branches)
        {
            //
            int action = (int)vectorAction[index];
            switch (bd.action_type){
                case "Throttle":
                    throttle = bd.branch_values[action];
                    break;
                case "Pitch":
                    pitch = bd.branch_values[action];
                    break;
                case "Roll":
                    roll = bd.branch_values[action];
                    break;
                case "Yaw":
                    yaw = bd.branch_values[action];
                    break;
                case "Flaps":
                    if (bd.branch_values[action]!=-1)
                        playerController.OnFlapsInput(bd.branch_values[action] == 1);
                    break;
                case "FireMissile":
                    if (bd.branch_values[action]!=-1)
                        playerController.OnFireMissile(bd.branch_values[action] == 1);
                    break;
                case "FireCannon":
                    if (bd.branch_values[action]!=-1)
                        playerController.OnFireCannon(bd.branch_values[action] == 1);
                    break;
            }
            index ++;
        }
        Vector3 RollPitchYaw = new Vector3(pitch,yaw,roll);
        aiController.SetSteeringInputs(new Steering(RollPitchYaw,throttle));
    }

    public override void Heuristic(float[] actionsOut)
    {   
        float dt = Time.time - t;        
        Steering steering = aiController.getSteering(dt);
        t = Time.time;
        int index = 0;
        int action;
        foreach (BranchData bd in config.ActionInfo.branches)
        {
            switch (bd.action_type){
                case "Throttle":
                    action = bd.branch_values.OrderBy(f => Mathf.Abs(steering.throttle - f)).First();
                    actionsOut[index] = bd.branch_values.IndexOf(action);
                    break;
                case "Pitch":
                    action = bd.branch_values.OrderBy(f => Mathf.Abs(steering.steering.x - f)).First();
                    actionsOut[index] = bd.branch_values.IndexOf(action);;
                    break;
                case "Yaw":
                    action = bd.branch_values.OrderBy(f => Mathf.Abs(steering.steering.y - f)).First();
                    actionsOut[index] = bd.branch_values.IndexOf(action);;
                    break;
                case "Roll":
                    action = bd.branch_values.OrderBy(f => Mathf.Abs(steering.steering.z - f)).First();
                    actionsOut[index] = bd.branch_values.IndexOf(action);;
                    break;
                case "Flaps":
                    actionsOut[index] = 0;
                    break;
                case "FireMissile":
                    actionsOut[index] = 0;
                    break;
                case "FireCannon":
                    actionsOut[index] = 0;
                    break;
            }
            index ++;
        }
        Debug.Log("steering Throttle: "+steering.throttle+" steering: "+steering.steering);
        Debug.Log(actionsOut.ToString());
    }

}
