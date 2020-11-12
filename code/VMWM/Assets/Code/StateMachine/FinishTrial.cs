using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrial : StateMachineBehaviour
{

    private Animator animator;
    private string status = "none";

    void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /* Log everything to .txt-file */
        ExperimentModel.logDataStore.logAllDataInStore();
        ExperimentModel.logDataStore.coordinateLog.Clear();
        this.animator = animator;

        if (ExperimentModel.runAccomplished() && ExperimentModel.remainingRun())
        {
            EventManager.TriggerEvent("showFinishRunPanel");
            EventManager.TriggerEvent("showFireworks"); 
            this.status = "runAccomplished";        
        }
        else if (ExperimentModel.runAccomplished() && !ExperimentModel.remainingRun())
        {
            EventManager.TriggerEvent("showFireworks");
            this.status = "experimentAccomplished";
        }
        else
        {
            EventManager.TriggerEvent("showFinishTrialPanel");
            this.status = "trialNotAccomplished";
        }      
    }
    void OnStateExit()
    {
        EventManager.TriggerEvent("hideFinishTrialPanel");
        EventManager.TriggerEvent("hideFinishRunPanel");
        EventManager.TriggerEvent("hideFireworks");
        this.status = "none";
    }
    void OnStateUpdate()
    {
        if ((Input.GetButtonDown("Confirm")) && status == "trialNotAccomplished")
        {
            EventManager.TriggerEvent("showDarkPanel");
            animator.SetTrigger("trialNotAccomplished");
        }
        else if ((Input.GetButtonDown("Confirm")) && status == "runAccomplished")
        {
            EventManager.TriggerEvent("showDarkPanel");
            animator.SetTrigger("runAccomplished");
        }
        else if (status == "experimentAccomplished")
        {
            animator.SetTrigger("experimentAccomplished");
        }
    }
}
