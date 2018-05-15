using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour {

    public GameObject soccerBall;
    public GameObject dribbleTarget;
    public Vector3 ballKickVector;
    public Vector3 ballTouchPoint;

    public float ballTouchDistance = 0.1F;

	// Use this for initialization
	void Start () {
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
        Debug.Log(new Vector3(2,2,2) - new Vector3(3,3,3));
	}
	
	// Update is called once per frame
	void Update () {
        if (soccerBall) {
            Vector3 tacticTargetVector = projectPointOntoFloor(transform.position) - projectPointOntoFloor(soccerBall.transform.position);
            Debug.DrawRay(projectPointOntoFloor(soccerBall.transform.position), tacticTargetVector, Color.green);

            if (dribbleTarget){
                Vector3 tacticPointVector = projectPointOntoFloor(dribbleTarget.transform.position) - projectPointOntoFloor(soccerBall.transform.position);
                ballKickVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
                ballTouchPoint = soccerBall.transform.position + ballKickVector;
                Debug.DrawRay(projectPointOntoFloor(soccerBall.transform.position), ballKickVector, Color.yellow);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(ballTouchPoint, "Light Gizmo.tiff", true);
    }

    Vector3 projectPointOntoFloor(Vector3 pointToProject) {
        Vector3 projectedPoint = new Vector3(pointToProject.x, 0, pointToProject.z);
        
        return projectedPoint;
    }
}
