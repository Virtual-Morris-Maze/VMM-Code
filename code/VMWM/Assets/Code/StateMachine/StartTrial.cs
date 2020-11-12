using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrial : StateMachineBehaviour {

    private Animator animator;
    private bool allowX = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        EventManager.StartListening("startTrial", () => {
            EventManager.TriggerEvent("showStartTrialPanel");
            EventManager.TriggerEvent("hideDarkPanel");
            allowX = true;
        });
        ExperimentModel.nextTrial();

        /* Logging */
        BasicLogger.newFile("Trial" + ExperimentModel.trial + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".txt");
        BasicLogger.openFile("Trial" + ExperimentModel.trial + "_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".txt");
    }
    void OnStateExit()
    {
        EventManager.TriggerEvent("hideCountDownPanel");
    }

    void OnStateUpdate()
    {
        if ((Input.GetButtonDown("Start") && allowX))
        {
            allowX = false;
            EventManager.TriggerEvent("hideStartTrialPanel");
            EventManager.TriggerEvent("showCountDownPanel");
            EventManager.StartListening("countDownPanelCountDownCompleted", () =>
            {
                animator.SetTrigger("trigger");
            });
            EventManager.TriggerEvent("countDownPanelCountDown");
        }
    }
}
