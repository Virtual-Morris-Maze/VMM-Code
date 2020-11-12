using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrial : StateMachineBehaviour {

    private Animator animator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        EventManager.TriggerEvent("enablePlayerMovement");
        EventManager.StartListening("platformFound", platformFound_Handler);
        ExperimentModel.startDurationLogging();

        /* Logging */
        EventManager.TriggerEvent("startCoordinateLogging");
    }
    void OnStateExit()
    {
        EventManager.TriggerEvent("stopCoordinateLogging");
    }

    void platformFound_Handler() {
        ExperimentModel.stopDurationLogging();
        animator.SetTrigger("trigger");
        EventManager.StopListening("platformFound", platformFound_Handler);
        EventManager.TriggerEvent("disablePlayerMovement");
    }

    void OnStateUpdate()
    {  
        if (Input.GetButtonDown("Pause")) {
            animator.SetTrigger("trialPause");
        } 
    }
}
