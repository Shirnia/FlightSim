using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
public class ObservationParser {


    public enum parents{Both,Agent,Target}; 
    public enum observation_type {
        Positions,Velocities,EulerAngles,EulerVelocities,WeaponSystems,
        HealthSystems,GForces,MissileLockDir,IncomingMissile,
        IncomingMissilePos, IncomingMissileDist, IncomingMissileAngle};
    FlightAgent agent;
    Plane player;
    Plane target;
    public ObservationParser(FlightAgent agent){
        this.agent = agent;
        this.player = agent.playerPlane;
        this.target = agent.targetPlane;
    }
    public List<float> ParseObs(ObservationInfo oi){
        parents parent = (parents)Enum.Parse(typeof(parents), oi.parent);
        observation_type obs_type = (observation_type)Enum.Parse(
            typeof(observation_type), oi.observation_type);
        //Get values
        List<Plane> planes = GetPlanes(parent);
        List<float> obs  = GetObservations(obs_type, planes, oi);

        return obs;
    }

    List<float> GetObservations(observation_type type, List<Plane> planes,
                                    ObservationInfo oi){
        List<float> obs = new List<float>();

        foreach(Plane plane in planes){
            Missile  incomingMissile = plane.Target.GetIncomingMissile();                    
            switch (type){
                case observation_type.Positions:
                    obs.AddRange(Vector3ToObs(plane.Rigidbody.position,oi));
                    break;
                case observation_type.Velocities:
                    obs.AddRange(Vector3ToObs(plane.Rigidbody.velocity,oi));
                    break;
                case observation_type.EulerAngles:
                    obs.AddRange(Vector3ToObs(plane.transform.eulerAngles,oi));
                    break;
                case observation_type.EulerVelocities:
                    obs.AddRange(Vector3ToObs(plane.Rigidbody.angularVelocity,oi));
                    break;
                case observation_type.WeaponSystems:
                    obs.Add(plane.MissileLocked? 1:0);
                    obs.Add(plane.MissileTracking? 1:0);
                    obs.Add(plane.cannonFiring? 1:0);
                    break;
                case observation_type.HealthSystems:
                    obs.Add(plane.Health/plane.MaxHealth);
                    break;
                case observation_type.GForces:
                    obs.Add(FloatToObs(plane.LocalGForce.y/9.81f,oi));
                    break;
                case observation_type.MissileLockDir:
                    obs.AddRange(Vector3ToObs(plane.MissileLockDirection,oi));
                    break;
                case observation_type.IncomingMissile:
                    obs.Add((incomingMissile != null)? 1:0);
                    break;                
                case observation_type.IncomingMissilePos:
                    if (incomingMissile!=null){
                        obs.AddRange(Vector3ToObs(
                            plane.transform.InverseTransformPoint(incomingMissile.Rigidbody.position),oi));
                    } else{
                        obs.Add(0);
                        obs.Add(0);
                        obs.Add(0);
                    }                    
                    break;
                case observation_type.IncomingMissileDist:
                    if (incomingMissile){
                        obs.Add(FloatToObs((incomingMissile.Rigidbody.position - plane.Rigidbody.position).magnitude,oi));
                    }
                    else
                        obs.Add(0);
                    break;
                case observation_type.IncomingMissileAngle:           
                    if (incomingMissile){
                        var missileDir = (incomingMissile.Rigidbody.position - plane.Rigidbody.position).normalized;
                        var missileAngle = Vector3.Angle(plane.transform.forward, missileDir);
                        obs.Add(FloatToObs(missileAngle,oi));
                    }else{
                        obs.Add(0);
                    }
                    break;

            }
        }

        return obs;
    }
    float FloatToObs(float val, ObservationInfo oi){
        float min = oi.min_max_values[0];
        float max = oi.min_max_values[1];
        return (val-min)/(max-min);
    }
    List<float> Vector3ToObs(Vector3 vec, ObservationInfo oi){
        List<float> obs = new List<float>();
        float min = oi.min_max_values[0];
        float max = oi.min_max_values[1];
        for (int i = 0; i<3; i++)
            obs.Add((vec[i]-min)/(max-min));
        return obs;
    }


    List<Plane> GetPlanes(parents parent){
        List<Plane> planes = new List<Plane>();
        switch (parent){
            case parents.Both:
                planes.Add(player);
                planes.Add(target);
                break;
            case parents.Agent:
                planes.Add(player);
                break;
            case parents.Target:
                planes.Add(target);
                break;
            default:
                Debug.LogError("Incorrect Parent for observations");
                break;
        }
        return planes;
    }
}
