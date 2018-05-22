using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.ootii.Messages;

public class SoccerPlayer : MonoBehaviour {

    public SoccerWorld soccerWorld;
    public GameObject dribbleTarget;
    public GameObject moveTarget;
    public GameObject ballProjection;
    public GameObject tacticPoint;
    public GameObject route;
    public GameObject wayPoint;
    public GameObject ballKickPointWithRunRelative;
    public GameObject ballKickPointWithRunPositionedRig;
    public GameObject kickDirectionWithRun;
    public Vector3 ballHitVector;
    public Vector3 ballTouchPoint;
    public Animator soccerPlayerAnimator;
    public float currentSpeed = 1F;
    public float finalSpeed = -1;
    public int playerKickMode = BallKickMode.await;
    public bool goalKickLocked = false;
    public float wayPointArrivalDistance = 0.3F;
    public float soccerKickCorrectionRate = 0.005F;
    public float ballTouchDistance = 0.1F;
    public int ballTouchDistanceCoeff = 350;
    public float dribbleHitPower = 5F;
    public float dribbleMaxSpeed = 10F;
    public float dribbleForceClamp = 1.2F;
    public float goalKickWithRunPower = 10F;
    public float goalKickWithRunMaxSpeed = 20F;
    public float goalKickWithRunForceClamp = 0.05F;

    private NavMeshAgent soccerPlayerNavigator;


    // Use this for initialization
    void Start () {
        route = GameObject.FindGameObjectWithTag("Route");
        ballProjection = GameObject.FindGameObjectWithTag("BallProjection");
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        soccerWorld = GameObject.FindGameObjectWithTag("SoccerWorld").GetComponent<SoccerWorld>();
        moveTarget = GameObject.FindGameObjectWithTag("Target");
        soccerPlayerAnimator = gameObject.GetComponent<Animator>();
        soccerPlayerNavigator = gameObject.GetComponent<NavMeshAgent>();
        ballKickPointWithRunRelative = transform.Find("BallKickPointWithRun").gameObject;
        playerKickMode = BallKickMode.dribble;
        MessageDispatcher.AddListener("BALL_HIT", OnBallHit, true);
        MessageDispatcher.AddListener("GOAL_KICK_CLICK", OnGoalKickCLick, true);
    }

    // Update is called once per frame
    void Update () {
        if (finalSpeed > -1) {
            if (currentSpeed != finalSpeed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, finalSpeed, 0.1F);
                soccerPlayerNavigator.speed = currentSpeed;
            }
            else finalSpeed = -1;
        }

        if (soccerWorld.soccerBall) {
            Vector3 tacticTargetVector = ProjectPointOntoFloor(transform.position, 0) - ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0);

            Vector3 playerToBall = soccerWorld.soccerBall.transform.position - transform.position;
            Vector3 tacticPointToBall = tacticPoint.transform.position - soccerWorld.soccerBall.transform.position;
            float maneuverAngle = Vector3.SignedAngle(playerToBall, tacticPointToBall, Vector3.up);

            Debug.DrawRay(ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0), tacticTargetVector, Color.green);

            ballTouchDistance = Mathf.Abs(maneuverAngle) / ballTouchDistanceCoeff;

            if (playerKickMode == BallKickMode.approachWithRun) {
                Vector3 goalKickPointToBall = soccerWorld.soccerBall.transform.position - ballKickPointWithRunRelative.transform.position;
                moveTarget.transform.position = transform.position + goalKickPointToBall;

                float kickPointToBallAngle = Vector3.SignedAngle(
                    ballKickPointWithRunRelative.transform.position - transform.position,
                    soccerWorld.soccerBall.transform.position - transform.position,
                    Vector3.up
                    );

                float kickPointToBallDistance = Vector3.Distance(ballKickPointWithRunRelative.transform.position, soccerWorld.soccerBall.transform.position);

                if (Mathf.Abs(kickPointToBallAngle) < 5 && kickPointToBallDistance < 0.2) playerKickMode = BallKickMode.targetLockedWithRun;

                Debug.Log("Goal Kick Point to ball angle, distance " + kickPointToBallAngle + ", " + kickPointToBallDistance);

            } else if (dribbleTarget) {
                Vector3 tacticPointVector = ProjectPointOntoFloor(dribbleTarget.transform.position, 0) - ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0);
                ballHitVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
                ballTouchPoint = soccerWorld.soccerBall.transform.position + ballHitVector;

                moveTarget.transform.position = ProjectPointOntoFloor(ballTouchPoint, 0.01F);

                Debug.DrawRay(ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0), ballHitVector, Color.yellow);
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

        if (playerKickMode == BallKickMode.targetLockedWithRun)
        {
            PerformGoalKick();
        }

        if (ballKickPointWithRunPositionedRig)
        {
            ballKickPointWithRunPositionedRig.transform.position = Vector3.Slerp(
                ballKickPointWithRunPositionedRig.transform.position,
                ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position,0), 
                soccerKickCorrectionRate);
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
        soccerPlayerAnimator.SetTrigger("GoalKick");
        //finalSpeed = 0;
        soccerPlayerNavigator.speed = 0;
        ballKickPointWithRunPositionedRig = new GameObject(gameObject.name);
        ballKickPointWithRunPositionedRig.tag = "Player";
        ballKickPointWithRunPositionedRig.transform.position = ProjectPointOntoFloor(ballKickPointWithRunRelative.transform.position,0);
        transform.parent = ballKickPointWithRunPositionedRig.transform;
        playerKickMode = BallKickMode.animateWithRun;
        //MessageDispatcher.SendMessage(this, "GOAL_KICK", gameObject.name, 0);
    }

    void OnBallHit(IMessage rMessage)
    {
        Debug.Log((string)rMessage.Data + " hits the ball");

        if(gameObject.name == (string)rMessage.Data)
        {
            //Perform ball hit
            if(playerKickMode == BallKickMode.dribble) {
                Vector3 goalKickDirection = tacticPoint.transform.position - soccerWorld.soccerBall.transform.position;
                soccerWorld.soccerBallRigidBody.AddForce(goalKickDirection.normalized / dribbleForceClamp, ForceMode.Acceleration);
                Debug.DrawRay(soccerWorld.soccerBall.transform.position, goalKickDirection.normalized, Color.red, 2F);
                Debug.Log("Applied force for dribble kick");

            } else if(playerKickMode == BallKickMode.animateWithRun) {
                Vector3 goalKickDirection = kickDirectionWithRun.transform.position - soccerWorld.soccerBall.transform.position;
                //Setup ball physics
                soccerWorld.soccerBallLogic.maxSpeed = goalKickWithRunMaxSpeed;
                soccerWorld.soccerBallLogic.forceClamp = goalKickWithRunForceClamp;
                //Stop correction
                playerKickMode = BallKickMode.dribble;
                //Kick the ball
                soccerWorld.soccerBallRigidBody.AddForce(goalKickDirection.normalized * goalKickWithRunPower, ForceMode.Impulse);
                //Dissasemble corrective rig
                GameObject soccerKickPointTmp = transform.parent.gameObject;
                transform.parent = null;
                Destroy(soccerKickPointTmp);

                Debug.DrawRay(soccerWorld.soccerBall.transform.position, goalKickDirection, Color.red, 2F);
                Debug.Log("Applied force for goal kick with run");
            }
        }
    }

    void OnGoalKickCLick(IMessage rMessage) {
        playerKickMode = BallKickMode.approachWithRun;
        Debug.Log(gameObject.name + " approaching for goal kick");
    }
}
