using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.ootii.Messages;

public class SoccerPlayer : MonoBehaviour {

    public GameObject soccerBall;
    public GameObject dribbleTarget;
    public GameObject moveTarget;
    public GameObject ballProjection;
    public GameObject tacticPoint;
    public GameObject route;
    public GameObject wayPoint;
    public GameObject goalKickPointRelative;
    public GameObject goalKickPointPositioned;
    public Vector3 ballHitVector;
    public Vector3 ballTouchPoint;
    public Animator m_Animator;
    public float currentSpeed = 1F;
    public float finalSpeed = -1;
    public bool goalKickMode = false;
    public bool goalKickLocked = false;
    public float wayPointArrivalDistance = 0.3F;
    public float soccerKickCorrectionRate = 0.005F;

    public int ballProjectionRotationSpeed = 10;

    public float ballTouchDistance = 0.1F;

    public int ballTouchDistanceCoeff = 350;

    public bool dribbleStraightforward = false;

    public float dribbleStraightForwardDeviation = 20F;

    private NavMeshAgent m_Navigator;


    // Use this for initialization
    void Start () {
        route = GameObject.FindGameObjectWithTag("Route");
        ballProjection = GameObject.FindGameObjectWithTag("BallProjection");
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
        moveTarget = GameObject.FindGameObjectWithTag("Target");
        m_Animator = gameObject.GetComponent<Animator>();
        m_Navigator = gameObject.GetComponent<NavMeshAgent>();

        goalKickPointRelative = transform.Find("GoalKickPoint").gameObject;
        MessageDispatcher.AddListener("BALL_HIT", OnBallHit, true);
    }

    // Update is called once per frame
    void Update () {
        if (finalSpeed > -1) {
            if (currentSpeed != finalSpeed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, finalSpeed, 0.1F);
                m_Navigator.speed = currentSpeed;
            }
            else finalSpeed = -1;
        }

        if (soccerBall) {
            Vector3 tacticTargetVector = ProjectPointOntoFloor(transform.position, 0) - ProjectPointOntoFloor(soccerBall.transform.position, 0);

            Vector3 playerToBall = soccerBall.transform.position - transform.position;
            Vector3 tacticPointToBall = tacticPoint.transform.position - soccerBall.transform.position;
            float maneuverAngle = Vector3.SignedAngle(playerToBall, tacticPointToBall, Vector3.up);

            Debug.DrawRay(ProjectPointOntoFloor(soccerBall.transform.position, 0), tacticTargetVector, Color.green);

            ballTouchDistance = Mathf.Abs(maneuverAngle) / ballTouchDistanceCoeff;

            if (goalKickMode) {
                Vector3 goalKickPointToBall = soccerBall.transform.position - goalKickPointRelative.transform.position;
                moveTarget.transform.position = transform.position + goalKickPointToBall;

                float kickPointToBallAngle = Vector3.SignedAngle(
                    goalKickPointRelative.transform.position - transform.position,
                    soccerBall.transform.position - transform.position,
                    Vector3.up
                    );

                float kickPointToBallDistance = Vector3.Distance(goalKickPointRelative.transform.position, soccerBall.transform.position);

                if (Mathf.Abs(kickPointToBallAngle) < 5 && kickPointToBallDistance < 0.2) goalKickLocked = true;

                Debug.Log("Goal Kick Point to ball angle, distance " + kickPointToBallAngle + ", " + kickPointToBallDistance);

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

        if (goalKickMode && goalKickLocked)
        {
            PerformGoalKick();
        }

        if (goalKickPointPositioned)
        {
            goalKickPointPositioned.transform.position = Vector3.Slerp(
                goalKickPointPositioned.transform.position,
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
        //finalSpeed = 0;
        m_Navigator.speed = 0;
        goalKickPointPositioned = new GameObject("SoccerKickPointGlobal");
        goalKickPointPositioned.name = gameObject.name;
        goalKickPointPositioned.tag = "Player";
        goalKickPointPositioned.transform.position = ProjectPointOntoFloor(goalKickPointRelative.transform.position,0);
        transform.parent = goalKickPointPositioned.transform;
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
