using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class LoadObservations 
{
    public ObservationInfo observationInfo;

    public LoadObservations()
    {
        string data =  Application.dataPath+"/Config/ObservationConfig.json";

        if(System.IO.File.Exists(data))
        {
            string json_txt = File.ReadAllText(data);
            observationInfo = JsonUtility.FromJson<ObservationInfo>(json_txt);
        } 
        else
        {
            Debug.LogError("No Configuration in the location "+data);
        }
    }
}

public enum Parent { Agent, Target, Missile }

public enum ObservationType { Positions, Velocities, EulerAngles, WeaponSystems, HealthSystems }

[System.Serializable]
public class ObservationInfo
{
    public string parent;
    public string observation_type;
    public int number_values;
    public List<int> min_max_values;
    public List<string> description;
}