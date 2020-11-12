using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentStartInputHandler : MonoBehaviour {

    public void setVP(string VP)
    {
        Debug.Log(VP);
        ExperimentModel.logDataStore.VP = VP;
    }

    public void setDescription(string description)
    {
        Debug.Log(description);
        ExperimentModel.logDataStore.description = description;
    }
}
