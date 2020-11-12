using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTrial : StateMachineBehaviour
{

    private Animator animator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("+ pauseTrial");
        this.animator = animator;
        EventManager.TriggerEvent("disablePlayerMovement");
        EventManager.TriggerEvent("showPauseTrialPanel");
        ExperimentModel.stopDurationLogging();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.TriggerEvent("hidePauseTrialPanel");
    }

    void OnStateUpdate()
    {
        if (Input.GetButtonDown("Pause"))
        {
            animator.SetTrigger("trialPause");
        }
    }
}
