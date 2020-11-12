using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishExperiment : StateMachineBehaviour
{
    private bool allowX = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        EventManager.TriggerEvent("showFinishExperimentPanel");
        EventManager.TriggerEvent("showDarkPanel");
    }
    void OnStateUpdate()
    {
        if ((Input.GetButtonDown("Confirm") && allowX))
        {
            Application.Quit();
        }
    }
}
