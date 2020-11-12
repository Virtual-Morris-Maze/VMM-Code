using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.PostProcessing;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private bool VREnabled => UnityEngine.XR.XRSettings.enabled;

    public GameObject canvas;
    public GameObject player;
    public GameObject darkPanel;
    public GameObject fireworks;

    public GameObject startTrialPanelNormal;
    public GameObject startTrialPanelVR;
    public GameObject startTrialPanel => VREnabled ? startTrialPanelVR : startTrialPanelNormal;

    public GameObject pauseTrialPanelNormal;
    public GameObject pauseTrialPanelVR;
    public GameObject pauseTrialPanel => VREnabled ? pauseTrialPanelVR : pauseTrialPanelNormal;

    public GameObject finishedTrialPanelNormal;
    public GameObject finishedTrialPanelVR;
    public GameObject finishedTrialPanel => VREnabled ? finishedTrialPanelVR : finishedTrialPanelNormal;

    public GameObject finishedRunPanelNormal;
    public GameObject finishedRunPanelVR;
    public GameObject finishedRunPanel => VREnabled ? finishedRunPanelVR : finishedRunPanelNormal;

    public GameObject startRunPanelNormal;
    public GameObject startRunPanelVR;
    public GameObject startRunPanel => VREnabled ? startRunPanelVR : startRunPanelNormal;

    public GameObject startExperimentPanel;
    public GameObject finishExperimentPanelNormal;
    public GameObject finishExperimentPanelVR;
    public GameObject finishExperimentPanel => VREnabled ? finishExperimentPanelVR : finishExperimentPanelNormal;

    public GameObject countDownPanelNormal;
    public GameObject countDownPanelVR;
    public GameObject countDownPanel => VREnabled ? countDownPanelVR : countDownPanelNormal;


    public PostProcessingProfile postProcessingProfile;

    private float tmpSaturation = 1.0f;
    private float tmpCanvasAlpha = 0.0f;
    private float tmpDarkPanelAlpha = 0.0f;

    //private float panelFadingValue = 0.0f;

    private bool trialFinishedUiActive = false;

    private float timerTime = 3.0f;
    private bool timerStarted = false;

    public UIController()
    {
    }

    void OnApplicationQuit()
    {
        BasicLogger.closeFile();
    }
    public void Start()
    {
        var settings = postProcessingProfile.colorGrading.settings;
        settings.basic.saturation = 0.0f;
        postProcessingProfile.colorGrading.settings = settings;

        /* Panel Show/Hide Events */
        EventManager.StartListening("hideDarkPanel", hideDarkPanel_Handler);
        EventManager.StartListening("showDarkPanel", showDarkPanel_Handler);
        EventManager.StartListening("hideStartExperimentPanel", hideStartExperimentPanel_Handler);
        EventManager.StartListening("showStartExperimentPanel", showStartExperimentPanel_Handler);
        EventManager.StartListening("hideFinishExperimentPanel", hideFinishExperimentPanel_Handler);
        EventManager.StartListening("showFinishExperimentPanel", showFinishExperimentPanel_Handler);
        EventManager.StartListening("hideStartRunPanel", hideStartRunPanel_Handler);
        EventManager.StartListening("showStartRunPanel", showStartRunPanel_Handler);
        EventManager.StartListening("hideStartTrialPanel", hideStartTrialPanel_Handler);
        EventManager.StartListening("showStartTrialPanel", showStartTrialPanel_Handler);
        EventManager.StartListening("hideCountDownPanel", hideCountDownPanel_Handler);
        EventManager.StartListening("showCountDownPanel", showCountDownPanel_Handler);
        EventManager.StartListening("hideFinishTrialPanel", hideFinishTrialPanel_Handler);
        EventManager.StartListening("showFinishTrialPanel", showFinishTrialPanel_Handler);
        EventManager.StartListening("hideFinishRunPanel", hideFinishRunPanel_Handler);
        EventManager.StartListening("showFinishRunPanel", showFinishRunPanel_Handler);
        EventManager.StartListening("hidePauseTrialPanel", hidePauseTrialPanel_Handler);
        EventManager.StartListening("showPauseTrialPanel", showPauseTrialPanel_Handler);

        /* Logic Events */
        EventManager.StartListening("countDownPanelCountDown", countDownPanelCountDown_Handler);
        EventManager.StartListening("showFireworks", showFireworks_Handler);
        EventManager.StartListening("hideFireworks", hideFireworks_Handler);
    }

    private void Update()
    {
        if ((Input.GetButtonDown("Escape")))
        {
            Application.Quit();
        }

        if (timerStarted == true)
        {
            timerTime -= Time.deltaTime;

            if (timerTime <= 1.0f)
            {
                this.timerStarted = false;
                EventManager.TriggerEvent("countDownPanelCountDownCompleted");
                this.timerTime = 3.0f;
            }
        }
    }

    private void showPauseTrialPanel_Handler()
    {
        this.togglePanel(pauseTrialPanel, true);
    }

    private void hidePauseTrialPanel_Handler()
    {
        this.togglePanel(pauseTrialPanel, false);
    }

    void showFireworks_Handler() {
        if (ExperimentModel.getFireworksVisibility())
        {
            this.fireworks.SetActive(true);
        }
    }

    void hideFireworks_Handler()
    {
        this.fireworks.SetActive(false);
    }

    void countDownPanelCountDown_Handler()
    {
        timerTime = ExperimentModel.getWaitTimeBeforeTrial();
        this.timerStarted = true;
    }

    void hideFinishRunPanel_Handler()
    {
        this.togglePanel(finishedRunPanel, false);
    }
    void showFinishRunPanel_Handler()
    {
        this.togglePanel(finishedRunPanel, true);
        finishedRunPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
            "Congratulations!\nYou finished run no. " + ExperimentModel.run;
    }
    void hideFinishTrialPanel_Handler()
    {
        this.togglePanel(finishedTrialPanel, false);
    }
    void showFinishTrialPanel_Handler()
    {
        finishedTrialPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
            "Congratulations!\nYou finished trial no. " + ExperimentModel.trial + ".";
        this.togglePanel(finishedTrialPanel, true);
        this.fadeSaturationOut();
    }

    void hideDarkPanel_Handler()
    {
        this.togglePanel(darkPanel,false);
    }
    void showDarkPanel_Handler()
    {
        this.togglePanel(darkPanel,true);
        this.fadeSaturationOut();
    }

    void hideCountDownPanel_Handler()
    {
        this.fadePanel(countDownPanel, false);
    }
    void showCountDownPanel_Handler()
    {
        this.togglePanel(countDownPanel, true);
        this.fadeSaturationIn();
    }
    void hideStartExperimentPanel_Handler()
    {
        this.togglePanel(startExperimentPanel,false);
    }
    void showStartExperimentPanel_Handler()
    {
        this.togglePanel(startExperimentPanel, true);
    }

    void hideFinishExperimentPanel_Handler()
    {
        this.togglePanel(finishExperimentPanel, false);
    }
    void showFinishExperimentPanel_Handler()
    {
        this.togglePanel(finishExperimentPanel, true);
    }

    void hideStartRunPanel_Handler()
    {
        this.togglePanel(startRunPanel, false);
    }
    void showStartRunPanel_Handler()
    {
        startRunPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Press <b>[Confirm]</b> to start run no. " + ExperimentModel.run + ".";
        this.togglePanel(startRunPanel, true);
    }

    void hideStartTrialPanel_Handler()
    {
        this.togglePanel(startTrialPanel, false);
    }
    void showStartTrialPanel_Handler()
    {
        startTrialPanel.GetComponentInChildren<TextMeshProUGUI>().text = "Press <b>[Start]</b> to start trial no. " + ExperimentModel.trial + ".";
        this.togglePanel(startTrialPanel, true);
    }


    private void OnGUI()
    {
        if (this.timerStarted)
        {
            TextMeshProUGUI tx = countDownPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            tx.text = "Your next trial starts in <b>" + (int) timerTime + "</b> seconds.";
            //this.countDownPanel.GetComponentInChildren<Text>().text = "Your next trial starts in " + ((int)(this.timerTime)).ToString() + " seconds...";
            if (timerTime <= 1.1f)
            {
                tx.text = "Go!";
            }
        }
    }

    private void togglePanel(GameObject panel, bool show, float duration = 1.0f) {
        if (show)
        {
            panel.SetActive(true);
        }
        else if (!show)
        {
            panel.SetActive(false);
        }
    }

    private void fadePanel(GameObject panel, bool show, float duration = 1.0f)
    {
        if (show)
        {
            float panelFadingValue = 0.0f;
            panel.SetActive(true);
            DOTween.To(() => panelFadingValue, x => panelFadingValue = x, 1.0f, duration).OnUpdate(() => { panel.GetComponent<CanvasGroup>().alpha = panelFadingValue; });
        }
        else if (!show)
        {
            float panelFadingValue = 1.0f;
            DOTween.To(() => panelFadingValue, x => panelFadingValue = x, 0.0f, duration).OnUpdate(() => { panel.GetComponent<CanvasGroup>().alpha = panelFadingValue; }).OnComplete(() => {
                panel.GetComponent<CanvasGroup>().alpha = 1.0f;
                panel.SetActive(false);
            });
        }
    }

    void fadeSaturationOut()
    {
        this.postProcessingProfile = this.player.GetComponentInChildren<PostProcessingBehaviour>().profile;
        DOTween.To(() => tmpSaturation, x => tmpSaturation = x, 0.0f, 1).OnUpdate(updateSaturation);
    }

    void fadeSaturationIn()
    {
        this.postProcessingProfile = this.player.GetComponentInChildren<PostProcessingBehaviour>().profile;
        DOTween.To(() => tmpSaturation, x => tmpSaturation = x, 1.0f, 1).OnUpdate(updateSaturation);
    }

    void updateSaturation()
    {
        var settings = postProcessingProfile.colorGrading.settings;
        settings.basic.saturation = this.tmpSaturation;
        postProcessingProfile.colorGrading.settings = settings;
    }
}