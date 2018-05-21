using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class SoccerPlayer : MonoBehaviour {

    public GameObject soccerBall;
    public GameObject dribbleTarget;
    public GameObject moveTarget;
    public GameObject ballProjection;
    public GameObject tacticPoint;
    public GameObject route;
    public GameObject wayPoint;
    public GameObject goalKickPointLocal;
    public GameObject goalKickPointGlobal;
    public Vector3 ballHitVector;
    public Vector3 ballTouchPoint;
    public Animator m_Animator;
    public bool goalKickMode = false;
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
        goalKickPointLocal = transform.Find("SoccerKickPoint").gameObject;
        MessageDispatcher.AddListener("BALL_HIT", OnBallHit, true);
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

            if (goalKickMode) {
                Vector3 goalKickPointToBall = soccerBall.transform.position - goalKickPointLocal.transform.position;
                moveTarget.transform.position = transform.position + goalKickPointToBall;

            } else if (dribbleTarget) {
                Vector3 tacticPointVector = ProjectPointOntoFloor(dribbleTarget.transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);
                ballHitVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
                ballTouchPoint = soccerBall.transform.position + ballHitVector;

                moveTarget.transform.position = ProjectPointOntoFloor(ballTouchPoint, 0.01F);

                Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), ballHitVector, Color.yellow);
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

        if (goalKickMode)
        {
            //PerformGoalKick();
        }

        if (goalKickPointGlobal)
        {
            goalKickPointGlobal.transform.position = Vector3.Slerp(
                goalKickPointGlobal.transform.position,
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

    void PerformGoalKick()
    {
        m_Animator.SetTrigger("SoccerKick");
        goalKickPointGlobal = new GameObject("SoccerKickPointGlobal");
        goalKickPointGlobal.name = gameObject.name;
        goalKickPointGlobal.tag = "Player";
        goalKickPointGlobal.transform.position = ProjectPointOntoFloor(goalKickPointLocal.transform.position,0);
        transform.parent = goalKickPointGlobal.transform;
        goalKickMode = false;
        MessageDispatcher.SendMessage(this, "GOAL_KICK", gameObject.name, 0);
    }

    void OnBallHit(IMessage rMessage)
    {
        Debug.Log((string)rMessage.Data + " hits the ball");
        if(gameObject.name == (string)rMessage.Data && transform.parent != null)
        {
            GameObject soccerKickPointTmp = transform.parent.gameObject;
            transform.parent = null;
            Destroy(soccerKickPointTmp);
        }
    }
}
