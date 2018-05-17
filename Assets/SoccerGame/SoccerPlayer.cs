﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour {

    public GameObject soccerBall;
    public GameObject dribbleTarget;
    public GameObject moveTarget;
    public GameObject ballProjection;
    public GameObject tacticPoint;
    public Vector3 ballKickVector;
    public Vector3 ballTouchPoint;

    public float ballTouchDistance = 0.1F;

    public int ballTouchDistanceCoeff = 350;

    public bool dribbleStraightforward = false;

    // Use this for initialization
    void Start () {
        ballProjection = GameObject.FindGameObjectWithTag("BallProjection");
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
        moveTarget = GameObject.FindGameObjectWithTag("Target");
	}
	
	// Update is called once per frame
	void Update () {
        if (soccerBall) {
            Vector3 tacticTargetVector = ProjectPointOntoFloor(transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);
            float maneuverAngle = Vector3.SignedAngle(soccerBall.transform.position - transform.position, tacticPoint.transform.position - soccerBall.transform.position, Vector3.up);

            Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), tacticTargetVector, Color.green);

            ballTouchDistance = Mathf.Abs(maneuverAngle) / ballTouchDistanceCoeff;

            if (dribbleTarget) {
                Vector3 tacticPointVector = ProjectPointOntoFloor(dribbleTarget.transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);
                ballKickVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
                ballTouchPoint = soccerBall.transform.position + ballKickVector;

                if (!dribbleStraightforward) moveTarget.transform.position = ProjectPointOntoFloor(ballTouchPoint, 0.01F);
                else moveTarget.transform.position = ProjectPointOntoFloor(soccerBall.transform.position, 0.01F);

                Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), ballKickVector, Color.yellow);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Debug.Log("Dribble Straight Forward");
                dribbleStraightforward = true;
            }
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            {
                Debug.Log("Dribble Turns");
                dribbleStraightforward = false;
            }
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
