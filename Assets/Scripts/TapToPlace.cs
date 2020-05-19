﻿using System.Collections;
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

    private Pose placementPose;
    private bool placementPoseValid = false;
    private bool targetPlaced = false;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arSession = FindObjectOfType<ARSession>();
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
            if (Input.touchCount > 0)
            {
                float startTouchTime, endTouchTime;
                startTouchTime = endTouchTime = 0.0f;

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    startTouchTime = Time.time;
                    InstantiateObject(6);
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended && startTouchTime != 0.0f)
                {
                    endTouchTime = Time.time;
                    float deltaTouchTime = endTouchTime - startTouchTime;
                    
                }
            }
        }
    }

    private void PlaceTarget()
    {
        Instantiate(targetToPlace, placementPose.position, placementPose.rotation);
        targetPlaced = true;
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
}
