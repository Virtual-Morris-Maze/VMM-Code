using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PostProcessing;
using DG.Tweening;
using System.Globalization;

public class RunConstructor : MonoBehaviour {


    public GameObject player;
    public GameObject pond;
    public GameObject pondFloor;
    public GameObject platform;
    public GameObject platformSpotlight;
    public GameObject spatialCueTriangle;
    public GameObject spatialCueSquare;
    public GameObject spatialCueCircle;
    public GameObject spatialCueDiamond;
    public Material simpleSkybox;
    public Material nonsimpleSkybox;
    private CultureInfo numberFormat;
    
    // Use this for initialization
    void Start () {
        EventManager.StartListening("buildRun", buildRun);
        EventManager.StartListening("spawnPlayer", spawnPlayer);
        numberFormat = ExperimentModel.numberFormat;
    }
    
    void buildRun() {
        scalePond(float.Parse(ExperimentModel.getRunConfig()[4], numberFormat));
        placeCues();
        setCueVisibility();
        scaleAndPlacePlatform(float.Parse(ExperimentModel.getRunConfig()[1], numberFormat),
            float.Parse(ExperimentModel.getRunConfig()[2], numberFormat), 
            float.Parse(ExperimentModel.getRunConfig()[3], numberFormat), ExperimentModel.getRunConfig()[9]);
        setSkybox();
        setFOV();
        Debug.Log("Build Run");
        EventManager.TriggerEvent("runBuildFinished");
    }

    void scalePond(float diameter) {
        this.pond.transform.localScale = new Vector3(diameter, this.pond.transform.localScale.y, diameter);
        pondFloor.transform.localScale = new Vector3(diameter, 1, diameter);
    }

    void placeCues() {
        //NE
        this.spatialCueTriangle.transform.position = this.pond.transform.position;
        this.spatialCueTriangle.transform.position = new Vector3(this.spatialCueTriangle.transform.position.x + this.pond.transform.localScale.x * 0.4f, 
                                                          2.0f,
                                                          this.spatialCueTriangle.transform.position.z + this.pond.transform.localScale.z * 0.4f);

        //SE
        this.spatialCueSquare.transform.position = this.pond.transform.position;
        this.spatialCueSquare.transform.position = new Vector3(this.spatialCueSquare.transform.position.x + this.pond.transform.localScale.x * 0.4f,
                                                          2.0f,
                                                          this.spatialCueSquare.transform.position.z - this.pond.transform.localScale.z * 0.4f);

        //NW
        this.spatialCueCircle.transform.position = this.pond.transform.position;
        this.spatialCueCircle.transform.position = new Vector3(this.spatialCueCircle.transform.position.x - this.pond.transform.localScale.x * 0.4f,
                                                          2.0f,
                                                          this.spatialCueCircle.transform.position.z + this.pond.transform.localScale.z * 0.4f);

        //SW
        this.spatialCueDiamond.transform.position = this.pond.transform.position;
        this.spatialCueDiamond.transform.position = new Vector3(this.spatialCueDiamond.transform.position.x - this.pond.transform.localScale.x * 0.4f,
                                                          2.0f,
                                                          this.spatialCueDiamond.transform.position.z - this.pond.transform.localScale.z * 0.4f);
    }

    void setCueVisibility() {
        var showAnyCues = ExperimentModel.getRunConfig()[5].Equals("1");

        string[] cueVisiblity = ExperimentModel.getSpatialCueVisibilityInfo();

        string NE = cueVisiblity[0].Split('=')[1];
        string NW = cueVisiblity[1].Split('=')[1];
        string SW = cueVisiblity[2].Split('=')[1];
        string SE = cueVisiblity[3].Split('=')[1];

        this.spatialCueCircle.SetActive(showAnyCues && NW.Equals("1"));
        this.spatialCueTriangle.SetActive(showAnyCues && NE.Equals("1"));
        this.spatialCueSquare.SetActive(showAnyCues && SE.Equals("1"));
        this.spatialCueDiamond.SetActive(showAnyCues && SW.Equals("1"));
        
    }

    void scaleAndPlacePlatform(float radius, float degree, float diameter, string showPlatform) {
        platform.transform.localScale = new Vector3(diameter, platform.transform.localScale.y, diameter);
        platform.transform.position = new Vector3(radius * pond.transform.localScale.x * 0.5f * Mathf.Sin(degree * Mathf.PI / 180.0f), 
                                                       platform.transform.position.y, 
                                                       radius * pond.transform.localScale.x * 0.5f * Mathf.Cos(degree * Mathf.PI / 180.0f));
        
        if (showPlatform.Equals("1")) {
            //The goal is to be shown
            platformSpotlight.SetActive(true);
            platform.transform.position = new Vector3(platform.transform.position.x, 0.315f, platform.transform.position.z);
        }
        else
        {
            //The goal should not be shown
            platformSpotlight.SetActive(false);
            platform.transform.position = new Vector3(platform.transform.position.x, 0.05f, platform.transform.position.z);
        }
    }

    void spawnPlayer() {
        //0 North, 1 South, 2 East, 3 West
        int spawnPoint = ExperimentModel.generateSpawnPoint();
        this.player.transform.rotation = Quaternion.identity;
        float headHeight = 1;
        if (UnityEngine.XR.XRSettings.enabled)
        {
            headHeight = 1 + Mathf.Abs(Camera.main.transform.localPosition.y);
        }

        do
        {
            switch (spawnPoint) {
                case 1: //-z -> South
                    this.player.transform.position = new Vector3(0.0f, headHeight, -1 * this.pond.transform.localScale.z * 0.5f * 0.9f);
                    this.player.transform.RotateAround(this.player.transform.position, new Vector3(0, 1, 0), 0);
                    break;
                case 2: //+x -> East
                    this.player.transform.position = new Vector3(this.pond.transform.localScale.z * 0.5f * 0.9f, headHeight, 0.0f);
                    this.player.transform.RotateAround(this.player.transform.position, new Vector3(0, 1, 0), -90);
                    break;
                case 3: //-x -> West
                    this.player.transform.position = new Vector3(-1 * this.pond.transform.localScale.z * 0.5f * 0.9f, headHeight, 0.0f);
                    this.player.transform.RotateAround(this.player.transform.position, new Vector3(0, 1, 0), 90);
                    break;

                default: //+z -> North
                    this.player.transform.position = new Vector3(0.0f, headHeight, pond.transform.localScale.z * 0.5f * 0.9f);
                    this.player.transform.RotateAround(this.player.transform.position, new Vector3(0, 1, 0), 180);
                    break;
            }
            
            if (spawnPoint < 3)
            {
                spawnPoint = spawnPoint + 1;
            }
            else
            {
                spawnPoint = 0;
            }
            
            //do-while: moves player to next spawn, if spawn would be on the platform
        } while (float.Parse(ExperimentModel.getRunConfig()[3], numberFormat) + 0.6f >= 
                 Vector3.Distance(player.transform.position, platform.transform.position));

        EventManager.TriggerEvent("playerSpawned");
    }

    private void setSkybox()
    {
        if (ExperimentModel.getSimpleSkyboxEnabled())
        {
            RenderSettings.skybox = simpleSkybox;
        }
        else
        {
            RenderSettings.skybox = nonsimpleSkybox;
        }
    }

    private void setFOV()
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Camera.main.fieldOfView = ExperimentModel.getFOV();
        }
    }
}