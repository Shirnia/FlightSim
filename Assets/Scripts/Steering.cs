using UnityEngine;

public class Steering {
    //Pitch, Yaw, Roll
    public Vector3 steering; 
    public float throttle;

    public Steering(Vector3 steering, float throttle){
        this.steering = steering;
        this.throttle = throttle;
    }
}