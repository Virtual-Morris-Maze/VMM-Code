using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;
using UnityEngine;

public static class ExperimentModel {

    private const string CONFIG_FILE_LOCATION = "../config/VMWM-Experiment-Config.csv";
    private const char DELIMITER = ';';
    public const float LOG_FREQUENCY = 50.0f;

    private static string[][] experimentConfiguration;
    /*  Csv-Mapping: [0]-> Run_ID, 
     *               [1]-> Pos_Goal_Distance, 
     *               [2]-> Pos_Goal_Degree, 
     *               [3]-> Goal_Size, 
     *               [4]-> Pond_Size, 
     *               [5]-> Spatial_Cue_Presence, 
     *               [6]-> Time_Threshold,
     *               [7]-> Scripted_Spawn_Points,
     *               [8]-> Spatial_Cue_Visibility,
     *               [9]-> Show_Target_Platform
     *               [10]-> Show_Fireworks
     *               [11]-> Movement_Speed
     *               [12]-> Wait_Time_Before_Trial
     *               [13]-> Simple_Skybox
     *               [14]-> FOV_Degree
     */

    //array which holds the default values for all variables, to prevent project crashing, when variables are missing
    private static string[] defaultValues = {"0", "0.5", "180", "2", "10", "1", "12000", "", "NE=1,NW=1,SW=1,SE=1",
        "0", "0", "1", "5", "1", "60"};
    
    public static TrialLogDataStore logDataStore;
   
    public static string experimentName = "";
    public static string VP = "";
    public static string description = "";

    private static long lastTrialStartTime;
    public static float trialTimeDelta = 0;
    public static float trialTotalTime = 0;

    public static int run = 0;
    public static int trial = 0;
    

    public static Vector3 tmpSpawnCoordinates = new Vector3();
    public static Vector3 tmpPlatformCollisionCoordinates = new Vector3();
    
    public static CultureInfo numberFormat = CultureInfo.InvariantCulture;//.NumberFormat.Clone() as NumberFormatInfo;
    
    public static string[] getRunConfig() {

        return experimentConfiguration[run];
    }

    //returns given value if it is valid, otherwise returns default value 
    private static string getValidValue(string column, string value)
    {
        switch (column)
        { 
            case "Run_ID":
                //returns default if value is not an integer or is negative
                if (!int.TryParse(value, out _) 
                    || int.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[0]);
                    return defaultValues[0];
                }
                break;
            case "Pos_Goal_Distance":
                if (!float.TryParse(value, out _) || float.Parse(value, numberFormat) < 0 
                                                  || float.Parse(value, numberFormat) > 1)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[1]);
                    return defaultValues[1];
                }
                break;
            case "Pos_Goal_Degree":
                if (!float.TryParse(value, out _) || float.Parse(value, numberFormat) < 0 
                                                  || float.Parse(value, numberFormat) > 360)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[2]);
                    return defaultValues[2];
                }
                break;
            case "Goal_Size":
                if (!float.TryParse(value, out _) || float.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[3]);
                    return defaultValues[3];
                }
                break;
            case "Pond_Size":
                if (!float.TryParse(value, out _) || float.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[4]);
                    return defaultValues[4];
                }
                break;
            case "Spatial_Cue_Presence":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0 
                                                || int.Parse(value, numberFormat) > 1)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[5]);
                    return defaultValues[5];
                }
                break;
            case "Time_Threshold":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[6]);
                    return defaultValues[6];
                }
                break;
            case "Scripted_Spawn_Points":
                bool valid = true;
                for (int i = 0; i < value.Length; i++)
                {
                    if (i % 2 != 0)
                    {
                        if (!value[i].Equals(',')) valid = false;
                    }
                    else
                    {
                        if (!(value[i].Equals('N') ^ value[i].Equals('S') ^ value[i].Equals('E') ^
                              value[i].Equals('W'))) valid = false;
                    }
                }

                if (!valid)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[7]);
                    return defaultValues[7];
                }
                break;
            case "Spatial_Cue_Visibility":
                value = value.Replace(" ", String.Empty);
                //I'm sorry
                if (value.Length != 19 || !value[0].Equals('N') || !value[1].Equals('E') || !value[2].Equals('=')
                    || !(value[3].Equals('0') ^ value[3].Equals('1')) || !value[4].Equals(',')
                    || !value[5].Equals('N') || !value[6].Equals('W') || !value[7].Equals('=')
                    || !(value[8].Equals('0') ^ value[8].Equals('1')) || !value[9].Equals(',')
                    || !value[10].Equals('S') || !value[11].Equals('W') || !value[12].Equals('=')
                    || !(value[13].Equals('0') ^ value[13].Equals('1')) || !value[14].Equals(',')
                    || !value[15].Equals('S') || !value[16].Equals('E') || !value[17].Equals('=')
                    || !(value[18].Equals('0') ^ value[18].Equals('1')))
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[8]);
                    return defaultValues[8];
                }
                break;
            case "Show_Target_Platform":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0 
                                                || int.Parse(value, numberFormat) > 1)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[9]);
                    return defaultValues[9];
                }
                break;
            case "Show_Fireworks":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0 
                                                || int.Parse(value, numberFormat) > 1)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[10]);
                    return defaultValues[10];
                }
                break;
            case "Movement_Speed":
                if (!float.TryParse(value, out _) || float.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[11]);
                    return defaultValues[11];
                }
                break; 
            case "Wait_Time_Before_Trial":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[12]);
                    return defaultValues[12];
                }
                break;
            case "Simple_Skybox":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 0 
                                                || int.Parse(value, numberFormat) > 1)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[13]);
                    return defaultValues[13];
                }
                break;
            case "FOV_Degree":
                if (!int.TryParse(value, out _) || int.Parse(value, numberFormat) < 40 
                                                || int.Parse(value, numberFormat) > 140)
                {
                    Debug.Log(value + " is not a valid value for " + column + ". Value set to default of " +
                              defaultValues[14]);
                    return defaultValues[14];
                }
                break;
        }
        return value;
    }

    //writes all values from one column of the input array to the another column in experimentConfiguration
    private static void swapCols(int destCol, int sourceCol, string[][] sourceArray)
    {
        for (int rows = 1; rows < sourceArray.Length; rows++)
        {
            experimentConfiguration[rows][destCol] =
                getValidValue(experimentConfiguration[0][destCol], sourceArray[rows][sourceCol]);
        }
    }

    public static void loadExperimentConfig() {
        //old simpler version, where all variables need to be entered
        //experimentConfiguration = File.ReadAllLines(CONFIG_FILE_LOCATION).Select(l => l.Split(DELIMITER).ToArray()).ToArray();
        //numberFormat.NumberDecimalSeparator = ".";
        
        string[][] temp = File.ReadAllLines(CONFIG_FILE_LOCATION).Select(l => l.Split(DELIMITER).ToArray()).ToArray();
        
        //create array of same size as input array and fill it with default values
        experimentConfiguration = new string[temp.Length][];
        for (int i = 0; i < experimentConfiguration.Length; i++)
        {
            experimentConfiguration[i] = (string[]) defaultValues.Clone();
        }
        
        //sets first row to variable names
        experimentConfiguration[0] = new string[]{"Run_ID", "Pos_Goal_Distance", "Pos_Goal_Degree" , "Goal_Size" , "Pond_Size" ,
            "Spatial_Cue_Presence" , "Time_Threshold" , "Scripted_Spawn_Points" , "Spatial_Cue_Visibility" , 
            "Show_Target_Platform" , "Show_Fireworks" , "Movement_Speed" , "Wait_Time_Before_Trial" , "Simple_Skybox" , "FOV_Degree"};

        //resorts cols/variables, so not all variables have to be included in the csv file but hardcoded numbers do still work
        //(e.g. Movement_Speed is always [11], no matter how many variables were used)
        for (int cols = 0; cols < temp[0].Length; cols++)
        {
            switch (temp[0][cols])
            { 
                    case "Run_ID":
                        swapCols(0, cols, temp);
                        break;
                    case "Pos_Goal_Distance":
                        swapCols(1, cols, temp);
                        break;
                    case "Pos_Goal_Degree":
                        swapCols(2, cols, temp);
                        break;
                    case "Goal_Size":
                        swapCols(3, cols, temp);
                        break;
                    case "Pond_Size":
                        swapCols(4, cols, temp);
                        break;
                    case "Spatial_Cue_Presence":
                        swapCols(5, cols, temp);
                        break;
                    case "Time_Threshold":
                        swapCols(6, cols, temp);
                        break;
                    case "Scripted_Spawn_Points":
                        swapCols(7, cols, temp);
                        break;
                    case "Spatial_Cue_Visibility":
                        swapCols(8, cols, temp);
                        break;
                    case "Show_Target_Platform":
                        swapCols(9, cols, temp);
                        break;
                    case "Show_Fireworks":
                        swapCols(10, cols, temp);
                        break;
                    case "Movement_Speed":
                        swapCols(11, cols, temp);
                        break; 
                    case "Wait_Time_Before_Trial":
                        swapCols(12, cols, temp);
                        break;
                    case "Simple_Skybox":
                        swapCols(13, cols, temp);
                        break;
                    case "FOV_Degree":
                        swapCols(14, cols, temp);
                        break;
            }
        }
    }

    private static void runBuiltFinished() {
        /* Post Run Settings to DataStore */
        

        nextTrial();
    }

    private static void experimentFinished() { }

    public static void nextRun() {
        run++;
        trial = 0;

        /* Log run data */
        logDataStore.runID = experimentConfiguration[run][0];
        logDataStore.goalDistance = experimentConfiguration[run][1];
        logDataStore.goalDegree = experimentConfiguration[run][2];
        logDataStore.goalSize = experimentConfiguration[run][3];
        logDataStore.pondSize = experimentConfiguration[run][4];
        logDataStore.spatialCuePresence = experimentConfiguration[run][5];
        logDataStore.timethreshold = experimentConfiguration[run][6];
        logDataStore.scriptedSpawnPoints = experimentConfiguration[run][7];
        logDataStore.spatialCueVisibility = experimentConfiguration[run][8];
        logDataStore.showTargetPlatform = experimentConfiguration[run][9];
        logDataStore.showFireworks = experimentConfiguration[run][10];
        logDataStore.movementSpeed = experimentConfiguration[run][11];
        logDataStore.waitTimeBeforeTrial = experimentConfiguration[run][12];
        logDataStore.simpleSkybox = experimentConfiguration[run][13];
        logDataStore.fovDegree = experimentConfiguration[run][14];

        EventManager.TriggerEvent("buildRun");
        
        EventManager.TriggerEvent("spawnPlayer");
    }

    public static void nextTrial() {
        trial++;
        trialTotalTime = 0;
        if(trial > 1) EventManager.TriggerEvent("spawnPlayer");
        EventManager.TriggerEvent("startTrial");
    }

    private static string removeQuotationMarks(this string original)
    {
        var charSequence = original.Where(c => c != '"');
        return new string(charSequence.ToArray());
    }
    
    public static int generateSpawnPoint() {

        if (experimentConfiguration[run][7].Equals(""))
        {
            Debug.Log("random!");
            System.Random rnd = new System.Random();
            int spawnPoint = rnd.Next(0, 3);
            switch (spawnPoint)
            {
                case 0:
                    logDataStore.spawnPoint = "N";
                    break;
                case 1:
                    logDataStore.spawnPoint = "S";
                    break;
                case 2:
                    logDataStore.spawnPoint = "E";
                    break;
                case 3:
                    logDataStore.spawnPoint = "W";
                    break;
            }

            return spawnPoint;
        }
        else {
            string[] spawnPointSequence = experimentConfiguration[run][7].removeQuotationMarks().Split(',');
            string spawnPointDirection = spawnPointSequence[((trial) % spawnPointSequence.Length)];
            Debug.Log("predefined: " + spawnPointDirection);
            switch (spawnPointDirection)
            {
                case "N":
                    logDataStore.spawnPoint = "N";
                    return 0;
                case "S":
                    logDataStore.spawnPoint = "S";
                    return 1;
                case "E":
                    logDataStore.spawnPoint = "E";
                    return 2;
                case "W":
                    logDataStore.spawnPoint = "W";
                    return 3;
            }
        }
        return 1;
    }

    public static string[] getSpatialCueVisibilityInfo()
    {
        string[] spawnPointSequence = experimentConfiguration[run][8].removeQuotationMarks().Split(',');
        return spawnPointSequence;
    }

    public static bool remainingRun() {
        if (run < experimentConfiguration.Length - 1) {
            return true;
        }
        return false;
    }

    public static void startDurationLogging() {
        lastTrialStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public static void stopDurationLogging() {
        trialTotalTime += calculateDuration();
    }

    public static bool runAccomplished() {
        if (trialTotalTime <= float.Parse(experimentConfiguration[run][6], numberFormat)) {
            return true;
        }
        return false;
    }

    public static float trialDuration() {
        return trialTotalTime * 0.001f;
    }

    private static float calculateDuration() {
        return ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lastTrialStartTime);
    }

    public static bool getFireworksVisibility()
    {
        if (experimentConfiguration[run][10].Equals("1"))
        {
            return true;
        }
        return false;
    }

    public static float getMovementSpeed()
    {
        return float.Parse(experimentConfiguration[run][11], numberFormat);
    }

    public static int getWaitTimeBeforeTrial()
    {
        return int.Parse(experimentConfiguration[run][12], numberFormat);
    }

    public static bool getSimpleSkyboxEnabled()
    {
        if (experimentConfiguration[run][13].Equals("0"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static int getFOV()
    {
        return int.Parse(experimentConfiguration[run][14], numberFormat);
    }
}