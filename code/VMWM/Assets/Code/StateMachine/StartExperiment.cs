using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExperiment : StateMachineBehaviour
{
    private Animator animator;
    private bool allowX = true;

    void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ExperimentModel.logDataStore = new TrialLogDataStore();
        ExperimentModel.loadExperimentConfig();
        this.animator = animator;
    }
    void OnStateExit()
    {
        ExperimentModel.logDataStore.experimentName = "VP" + ExperimentModel.logDataStore.VP + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
        ExperimentModel.logDataStore.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        BasicLogger.newFolder("../Logs/" + ExperimentModel.logDataStore.experimentName);

        EventManager.TriggerEvent("hideStartExperimentPanel");
    }

    void OnStateUpdate() {
        if ((Input.GetButtonDown("Confirm") && allowX)) {
            allowX = false;
            animator.SetTrigger("trigger");
        }
    }
}