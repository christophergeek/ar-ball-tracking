using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class TapToPlace : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject targetToPlace;
    public Rigidbody objectToInstantiate;

    private ARSessionOrigin arOrigin;
    private ARSession arSession;
    private ARPlaneManager arPlaneManager;
    private ARPointCloudManager arPointCloudManager;

    private Pose placementPose;
    private bool placementPoseValid = false;

    private GameObject currentTarget;
    private bool targetPlaced = false;

    private bool startTouch = false;
    private float startTouchTime, endTouchTime;

    private bool planesAreVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arSession = FindObjectOfType<ARSession>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arPointCloudManager = FindObjectOfType<ARPointCloudManager>();

        startTouchTime = endTouchTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            if (placementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceTarget();
            }
        }
        else if (targetPlaced)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTouchTime = Time.time;
                startTouch = true;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended && startTouch)
            {
                endTouchTime = Time.time;
                startTouch = false;

                float deltaTouchTime = endTouchTime - startTouchTime;
                InstantiateObject(Math.Min(10.0f, 2+ 3*deltaTouchTime));
            }
        }
    }

    private void PlaceTarget()
    {
        currentTarget = Instantiate(targetToPlace, placementPose.position, placementPose.rotation);
        targetPlaced = true;
        // PlaneToggle();
    }

    private void InstantiateObject(float deltaTouchTime)
    {
        Rigidbody newObject = Instantiate(objectToInstantiate, transform.position, transform.rotation);
        newObject.velocity = Camera.current.transform.forward.normalized * deltaTouchTime;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);

        placementPoseValid = hits.Count > 0;
        if (placementPoseValid)
        {
            placementPose = hits[0].pose;

            var cameraUp = Camera.current.transform.up;
            var planeForward = hits[0].pose.up;

            var dotProduct = Vector3.Dot(cameraUp, planeForward);
            var cameraBearing = (cameraUp - dotProduct*planeForward).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing, planeForward);
        }
    }

    public void ResetTarget()
    {
        targetPlaced = false;
        startTouch = false;
        Destroy(currentTarget);
        // PlaneToggle();
    }

    public void PlaneToggle()
    {
        planesAreVisible = !planesAreVisible;

        //disables/enables prefabs for planes found in future
        arPlaneManager.planePrefab.SetActive(planesAreVisible);

        //disables/enables renderering existing planes
        foreach (GameObject plane in GameObject.FindGameObjectsWithTag("AR Plane"))
        {
            Renderer r = plane.GetComponent<Renderer>();
            ARPlaneMeshVisualizer t = plane.GetComponent<ARPlaneMeshVisualizer>();
            r.enabled = planesAreVisible;
            t.enabled = planesAreVisible;
        }

        //disables/enables prefabs for points found in future
        arPointCloudManager.pointCloudPrefab.SetActive(planesAreVisible);

        //disables/enables renderering existing points
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("AR Points"))
        {
            Renderer r = point.GetComponent<Renderer>();
            ARPointCloudParticleVisualizer t = point.GetComponent<ARPointCloudParticleVisualizer>();
            r.enabled = planesAreVisible;
            t.enabled = planesAreVisible;
        }

    }
}
