using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrialDurationDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<TextMeshProUGUI>().text = "Current trial duration: " + ExperimentModel.trialTotalTime * 0.001 + "s";
	}
}
