using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LoadConfig
{
    public ActionInfo ActionInfo;

    public LoadConfig()
    {
        string data =  Application.dataPath+"/Config/VectorActionConfig.json";

        if(System.IO.File.Exists(data))
        {
            string json_txt = File.ReadAllText(data);
            ActionInfo = JsonUtility.FromJson<ActionInfo>(json_txt);
        } 
        else
        {
            Debug.LogError("No Configuration in the location "+data);
        }
    }
}

[System.Serializable]
public class ActionInfo
{
    public string space_type;
    public List<BranchData> branches;
}

[System.Serializable]
public class BranchData
{
    public string action_type;
    public List<int> branch_values;
    public List<string> description;
}