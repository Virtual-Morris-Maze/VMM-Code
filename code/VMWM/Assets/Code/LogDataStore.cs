using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CoordinateSample
{
    public long timestamp;
    public float x;
    public float z;
}

public class TrialLogDataStore {
    public string VP = "";
    public string description = "";
    public string experimentName = "";
    public string date = "";
    public string runID = "";
    public string goalDistance = "";
    public string goalDegree = "";
    public string goalSize = "";
    public string pondSize = "";
    public string spatialCuePresence = "";
    public string timethreshold = "";
    public string scriptedSpawnPoints = "";
    public string spatialCueVisibility = "";
    public string showTargetPlatform = "";
    public string spawnPoint = "";
    public string showFireworks = "";
    public string movementSpeed = "";
    public string waitTimeBeforeTrial = "";
    public string simpleSkybox = "";
    public string fovDegree = "";
    public List<CoordinateSample> coordinateLog = new List<CoordinateSample>();
    public string trialDuration;

    public void logAllDataInStore() {
        /* Logging everything to Trial File*/
        BasicLogger.newLine("<vmwm-log>");
        /*global information */
        BasicLogger.newLine("<global-info>");
        BasicLogger.newLine("<name>" + ExperimentModel.logDataStore.experimentName + "</name>");
        BasicLogger.newLine("<vp>" + ExperimentModel.logDataStore.VP + "</vp>");
        BasicLogger.newLine("<description>" + ExperimentModel.logDataStore.description + "</description>");
        BasicLogger.newLine("<log-frequency>" + ExperimentModel.LOG_FREQUENCY.ToString() + "</log-frequency>");
        BasicLogger.newLine("<date>" + ExperimentModel.logDataStore.date.ToString() + "</date>");
        BasicLogger.newLine("</global-info>");
        /* Run specific logging */
        BasicLogger.newLine("<run-specific-data>");
        BasicLogger.newLine("<run-id>" + ExperimentModel.logDataStore.runID + "</run-id>");
        BasicLogger.newLine("<goal-distance>" + ExperimentModel.logDataStore.goalDistance + "</goal-distance>");
        BasicLogger.newLine("<goal-degree>" + ExperimentModel.logDataStore.goalDegree + "</goal-degree>");
        BasicLogger.newLine("<goal-size>" + ExperimentModel.logDataStore.goalSize + "</goal-size>");
        BasicLogger.newLine("<pond-size>" + ExperimentModel.logDataStore.pondSize + "</pond-size>");
        BasicLogger.newLine("<spatial-cue-presence>" + ExperimentModel.logDataStore.spatialCuePresence + "</spatial-cue-presence>");
        BasicLogger.newLine("<time-threshold>" + ExperimentModel.logDataStore.timethreshold + "</time-threshold>");
        BasicLogger.newLine("<scripted-spawn-points>" + ExperimentModel.logDataStore.scriptedSpawnPoints + "</scripted-spawn-points>");
        BasicLogger.newLine("<spatial-cue-visibility>" + ExperimentModel.logDataStore.spatialCueVisibility + "</spatial-cue-visibility>");
        BasicLogger.newLine("<show-target-platform>" + ExperimentModel.logDataStore.showTargetPlatform + "</show-target-platform>");
        BasicLogger.newLine("<show-fireworks>" + ExperimentModel.logDataStore.showFireworks + "</show-fireworks>");
        BasicLogger.newLine("<movement-speed>" + ExperimentModel.logDataStore.movementSpeed + "</movement-speed>");
        BasicLogger.newLine("<wait-time-before-trial>" + ExperimentModel.logDataStore.waitTimeBeforeTrial + "</wait-time-before-trial>");
        BasicLogger.newLine("<simple-skybox>" + ExperimentModel.logDataStore.simpleSkybox + "</simple-skybox>");
        BasicLogger.newLine("<fov-degree>" + ExperimentModel.logDataStore.fovDegree + "<fov-degree>");
        BasicLogger.newLine("</run-specific-data>");
        /* Trial specific logging */
        BasicLogger.newLine("<trial-specific-data>");
        BasicLogger.newLine("<spawn-point>" + ExperimentModel.logDataStore.spawnPoint + "</spawn-point>");
        BasicLogger.newLine("<duration>" + ExperimentModel.trialTotalTime + "</duration>");
        BasicLogger.newLine("<coordinate-log>");
        foreach (CoordinateSample coordinateSample in ExperimentModel.logDataStore.coordinateLog) {
            BasicLogger.newLine("<sample>");
            BasicLogger.newLine("<timestamp>" + coordinateSample.timestamp + "</timestamp>");
            BasicLogger.newLine("<coordinate>");
            BasicLogger.newLine("<x>" + coordinateSample.x.ToString() + "</x>");
            BasicLogger.newLine("<z>" + coordinateSample.z.ToString() + "</z>");
            BasicLogger.newLine("</coordinate>");
            BasicLogger.newLine("</sample>");
        }
        BasicLogger.newLine("<samples>" + ExperimentModel.logDataStore.coordinateLog.Count.ToString() + "</samples>");
        BasicLogger.newLine("</coordinate-log>");
        BasicLogger.newLine("</trial-specific-data>");
        BasicLogger.newLine("</vmwm-log>");

        BasicLogger.closeFile();
    }
}
