using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementMode
    {
        BodyRelatve,
        HeadRelative
    }

    public enum RotationMode
    {
        ContinousBody,
        StepBody,
        Disabled
    }

    //[Header("Reference to the gameobject of the player head, which is usually the main camera")]
    //public GameObject HeadObject;

    [Tooltip("The movement mode that is used for moving forwards and backswards")]
    public MovementMode PlayerMovementMode;

    [Tooltip("The rotation mode that is used for rotating the player")]
    public RotationMode PlayerRotationMode;

    [Tooltip("By how much the player is rotated in any direction when the step rotation mode is selected")]
    public float StepRotateBy;
    [Tooltip("By how much the horizontal axis has to be moved for a step rotation to take place")][Range(0,1)]
    public float StepMinimumAxisThreshold;

    private const float MOVE_FORCE = 1.7f;
    private DateTime UNIX_START_TIME = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private bool movingAllowed = false;
    private Transform headTransform;

    long lastUnixTimestamp = 0;
    float lastTimeElapsed = 0;

    private bool hasRotated;
    private Rigidbody rigidbody;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        
        EventManager.StartListening("disablePlayerMovement", () => { this.movingAllowed = false; });
        EventManager.StartListening("enablePlayerMovement", () => { this.movingAllowed = true; });

        ///* Logging */
        EventManager.StartListening("startCoordinateLogging", () => { InvokeRepeating("logCoordinate", 0.00001f, 1.0f / 50.0f); });
        EventManager.StartListening("stopCoordinateLogging", () => { CancelInvoke(); });

        headTransform = Camera.main.transform;
    }

    void logCoordinate() {
        CoordinateSample tmpSample = new CoordinateSample();
        tmpSample.x = transform.position.x;
        tmpSample.z = transform.position.z;
        tmpSample.timestamp = (long)System.DateTime.UtcNow.ToUniversalTime().Subtract(UNIX_START_TIME).TotalMilliseconds;

        lastUnixTimestamp = tmpSample.timestamp;
        lastTimeElapsed = Time.realtimeSinceStartup;

        ExperimentModel.logDataStore.coordinateLog.Add(tmpSample);
    }

    // Update is called once per frame
    void Update()
    {
        //rigidbody.isKinematic = !movingAllowed;
        if (movingAllowed)
        {
            UpdateMovement();

            UpdateRotation();

        }
    }

    private void UpdateMovement()
    {
        var forwardDelta = Input.GetAxis("Vertical") * Time.deltaTime * ExperimentModel.getMovementSpeed();

        Vector3 forwardVector; 
        
        if (PlayerMovementMode == MovementMode.BodyRelatve)
        {
            forwardVector = transform.forward * forwardDelta;
        }
        else
        {
            Vector3 fakeForward = headTransform.forward;
            fakeForward.y = 0;
            fakeForward.Normalize();
            forwardVector = fakeForward * forwardDelta;
        }
        
        if (UnityEngine.XR.XRSettings.enabled)
        {
            GetComponent<SphereCollider>().center = headTransform.localPosition;
        }
        
        transform.position += forwardVector;
    }

    private void UpdateRotation()
    {
        var rotateDelta = Input.GetAxis("Horizontal") * Time.deltaTime * 100;
        if (PlayerRotationMode == RotationMode.ContinousBody)
        {
            transform.RotateAround(headTransform.position, new Vector3(0, 1, 0), rotateDelta);
            //transform.Rotate(0, rotateDelta, 0);
        }
        else if (PlayerRotationMode == RotationMode.StepBody)
        {
            if (Mathf.Abs(rotateDelta) > StepMinimumAxisThreshold)
                hasRotated = false;
            else if (!hasRotated)
            {
                transform.RotateAround(headTransform.position, new Vector3(0, 1, 0), Mathf.Sign(rotateDelta) * StepRotateBy);
                //transform.Rotate(0, Mathf.Sign(rotateDelta) * StepRotateBy, 0);
                hasRotated = true;
            }
        }
    }
}