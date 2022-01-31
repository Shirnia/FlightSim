using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem; 
using System.Linq;


public class FlightAgent : Agent
{
    public Plane playerPlane;
    public Plane targetPlane;
    private PlayerController playerController;
    private AIController playerAIController;
    private LoadConfig config;
    private float t;
    [SerializeField]
    public int difficulty = 0;
    // Start is called before the first frame update

    void Start()
    {
        config = new LoadConfig();
        playerController = GetComponent<PlayerController>();
        playerAIController = playerPlane.GetComponent<AIController>();
    }

     public override void OnEpisodeBegin()
    {
        float max_radius = 2500;
        //Get XZ of player+target
        Vector2 player_xz = Random.insideUnitCircle*max_radius;
        Vector2 target_xz;
        do {
            target_xz = Random.insideUnitCircle*max_radius;
        } 
        while (Vector2.Distance(player_xz,target_xz) < 0.2*max_radius);
        //Get Y of plauer+target
        float player_y = Random.Range(1000,5000);
        float target_y = Random.Range(1000,5000);

        playerPlane.transform.position = new Vector3(player_xz[0],player_y,player_xz[1]);
        targetPlane.transform.position = new Vector3(target_xz[0],target_y,target_xz[1]);

        Vector2 lookAt = Random.insideUnitCircle*max_radius*2;
        playerPlane.transform.LookAt(new Vector3(lookAt[0],2500,lookAt[1]));
        targetPlane.transform.LookAt(new Vector3(lookAt[0],2500,lookAt[1]));
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
        playerAIController.SetSteeringInputs(new Steering(RollPitchYaw,throttle));
    }

    public override void Heuristic(float[] actionsOut)
    {   
        float dt = Time.time - t;        
        Steering steering = playerAIController.getSteering(dt);
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
                    if (playerAIController.CalculateMissiles(dt))
                    {
                        Debug.Log("firing missile");
                        actionsOut[index] = 2;
                    } else {
                        actionsOut[index] = 1;
                    }
                    break;
                case "FireCannon":
                    if (playerAIController.CalculateCannon(dt))
                    {
                        Debug.Log("firing missile");
                        actionsOut[index] = 2;
                    } else {
                        actionsOut[index] = 1;
                    }
                    break;
            }
            index ++;
        }
    }

}
