using System.Collections;
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
    public GameObject soccerKickPointLocal;
    public GameObject soccerKickPointGlobal;
    public Vector3 ballKickVector;
    public Vector3 ballTouchPoint;
    public Animator m_Animator;
    public bool kickMode = false;
    public float wayPointArrivalDistance = 0.3F;
    public float soccerKickCorrectionRate = 0.005F;

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
        m_Animator = gameObject.GetComponent<Animator>();
        soccerKickPointLocal = transform.Find("SoccerKickPoint").gameObject;
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
            if (route.transform.childCount > 0)
            {
                wayPoint = route.transform.GetChild(0).gameObject;
                tacticPoint.transform.position = wayPoint.transform.position;
            }
        }
        else if (Vector3.Distance(ballProjection.transform.position, wayPoint.transform.position) < wayPointArrivalDistance) {
            Destroy(wayPoint);
        }

        if (kickMode)
        {
            PerformSoccerKick();
        }

        if (soccerKickPointGlobal)
        {
            soccerKickPointGlobal.transform.position = Vector3.Slerp(
                soccerKickPointGlobal.transform.position,
                ProjectPointOntoFloor(soccerBall.transform.position,0), soccerKickCorrectionRate);
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

    void PerformSoccerKick()
    {
        m_Animator.SetTrigger("SoccerKick");
        soccerKickPointGlobal = new GameObject("SoccerKickPointGlobal");
        soccerKickPointGlobal.transform.position = ProjectPointOntoFloor(soccerKickPointLocal.transform.position,0);
        transform.parent = soccerKickPointGlobal.transform;
        kickMode = false;
    }
}
