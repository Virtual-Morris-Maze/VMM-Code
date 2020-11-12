using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRun : StateMachineBehaviour
{
    private Animator animator;
    private bool allowX = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        EventManager.StartListening("runBuildFinished", () => { allowX = true; });
        ExperimentModel.nextRun();
        EventManager.TriggerEvent("showStartRunPanel");

        /* Logging */
        if (BasicLogger.currentFolder() == "")
        {
            BasicLogger.newFolder("../Logs/" + ExperimentModel.logDataStore.experimentName + "/Run" + ExperimentModel.run + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));
            BasicLogger.switchFolder("../Logs/" + ExperimentModel.logDataStore.experimentName + "/Run" + ExperimentModel.run + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));
        }
        else {
            BasicLogger.newFolder("../../../Logs/" + ExperimentModel.logDataStore.experimentName + "/Run" + ExperimentModel.run + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));
            BasicLogger.switchFolder("../Logs/" + ExperimentModel.logDataStore.experimentName + "/Run" + ExperimentModel.run + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));
        }
    }
    void OnStateExit()
    {
        EventManager.TriggerEvent("hideStartRunPanel");
    }
    void OnStateUpdate()
    {
        if ((Input.GetButtonDown("Confirm") && allowX))
        {
            allowX = false;
            animator.SetTrigger("trigger");
        }
    }
}
