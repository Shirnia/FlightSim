using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class LoadObservations 
{
    public ObservationBase observationBase;

    public LoadObservations()
    {
        string data =  Application.dataPath+"/Config/ObservationConfig.json";

        if(System.IO.File.Exists(data))
        {
            string json_txt = File.ReadAllText(data);
            observationBase = JsonUtility.FromJson<ObservationBase>(json_txt);
        } 
        else
        {
            Debug.LogError("No Configuration in the location "+data);
        }
    }
}

public enum Parent { Agent, Target, Missile }

public enum ObservationType { Positions, Velocities, EulerAngles, WeaponSystems, HealthSystems }

public class ObservationBase{
    public List<ObservationInfo> branches;
}

[System.Serializable]
public class ObservationInfo
{
    public string parent;
    public string observation_type;
    public int number_values;
    public List<int> min_max_values;
    public List<string> description;
}