﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour {

    public GameObject soccerBall;
    public GameObject dribbleTarget;
    public GameObject moveTarget;
    public GameObject ballProjection;
    public GameObject tacticPoint;
    public GameObject route;
    public GameObject wayPoint;
    public Vector3 ballKickVector;
    public Vector3 ballTouchPoint;
    public float wayPointArrivalDistance = 0.3F;

    public int ballProjectionRotationSpeed = 10;

    public float ballTouchDistance = 0.1F;

    public int ballTouchDistanceCoeff = 350;

    public bool dribbleStraightforward = false;

    public float dribbleStraightForwardDeviation = 20F;

    // Use this for initialization
    void Start () {
        route = GameObject.FindGameObjectWithTag("Route");
        ballProjection = GameObject.FindGameObjectWithTag("BallProjection");
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
        moveTarget = GameObject.FindGameObjectWithTag("Target");
	}
	
	// Update is called once per frame
	void Update () {
        if (soccerBall) {
            Vector3 tacticTargetVector = ProjectPointOntoFloor(transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);

            Vector3 playerToBall = soccerBall.transform.position - transform.position;
            Vector3 tacticPointToBall = tacticPoint.transform.position - soccerBall.transform.position;
            float maneuverAngle = Vector3.SignedAngle(playerToBall, tacticPointToBall, Vector3.up);

            Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), tacticTargetVector, Color.green);

            ballTouchDistance = Mathf.Abs(maneuverAngle) / ballTouchDistanceCoeff;

            if (dribbleTarget) {
                Vector3 tacticPointVector = ProjectPointOntoFloor(dribbleTarget.transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);
                ballKickVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
                ballTouchPoint = soccerBall.transform.position + ballKickVector;

                if (!dribbleStraightforward) moveTarget.transform.position = ProjectPointOntoFloor(ballTouchPoint, 0.01F);
                else moveTarget.transform.position = Vector3.Slerp(
                    moveTarget.transform.position,
                    ProjectPointOntoFloor(soccerBall.transform.position, 0.01F), 0.1F);

                Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), ballKickVector, Color.yellow);
            }
        }

        if (route && !wayPoint)
        {
            wayPoint = route.transform.GetChild(0).gameObject;
            tacticPoint.transform.position = wayPoint.transform.position;
        }
        else if (Vector3.Distance(ballProjection.transform.position, wayPoint.transform.position) < wayPointArrivalDistance) {
            Destroy(wayPoint);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(ballTouchPoint, "Light Gizmo.tiff", true);
    }

    Vector3 ProjectPointOntoFloor(Vector3 pointToProject, float yOffset) {
        Vector3 projectedPoint = new Vector3(pointToProject.x, yOffset, pointToProject.z);
        
        return projectedPoint;
    }
}
