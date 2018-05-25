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
    public GameObject ballKickPointWithRunRelative;
    public GameObject ballKickPointPositionedRig;
    public float goalKickWithRunPower = 10F;
    public float goalKickWithRunMaxSpeed = 20F;
    public float goalKickWithRunForceClamp = 0.05F;
    public GameObject ballKickPointFastRelative;
    public GameObject ballKickPointFastPositionedRig;
    public float goalKickFastPower = 10F;
    public float goalKickFastMaxSpeed = 20F;
    public float goalKickFastForceClamp = 0.05F;
    public bool goalKickTargetLocked = false;
    public float goalKickForceTimeout = 0F;
    public float goalKickForceTimeoutLength = 1F;
    public float dribbleKickDirectionLerp = 0.5F;
    public float debugRayDuration = 1F;
    public bool debugUpdate = false;
    public GameObject ballHitAgent;
    public float ballHitAgentInteractDistance = 0.1F;

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
        ballKickPointFastRelative = transform.Find("BallKickPointFast").gameObject;
        playerKickMode = BallKickMode.dribble;
        MessageDispatcher.AddListener("BALL_HIT", OnBallHit, true);
        MessageDispatcher.AddListener("GOAL_KICK_CLICK", OnGoalKickCLick, true);
        MessageDispatcher.AddListener("GOAL_KICK_FAST_CLICK", OnGoalKickFastCLick, true);
    }

    // Update is called once per frame
    void Update () {
        if (goalKickForceTimeout > 0) goalKickForceTimeout = goalKickForceTimeout - Time.deltaTime;
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

            SetMoveTarget();
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

        if ((playerKickMode == BallKickMode.approachFast || playerKickMode == BallKickMode.approachWithRun) && goalKickTargetLocked) PerformGoalKick();
        if (playerKickMode == BallKickMode.animateFast || playerKickMode == BallKickMode.animateWithRun) {
            float hitAgentToBallDistance = Vector3.Distance(ballHitAgent.transform.position, soccerWorld.soccerBall.transform.position);
            if (hitAgentToBallDistance < ballHitAgentInteractDistance) ApplyGoalKickForce();
        }

        if (ballKickPointPositionedRig)
        {
            ballKickPointPositionedRig.transform.position = Vector3.Slerp(
                ballKickPointPositionedRig.transform.position,
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
        //finalSpeed = 0;
        soccerPlayerNavigator.speed = 0;
        ballKickPointPositionedRig = new GameObject(gameObject.name);
        ballKickPointPositionedRig.tag = "Player";

        if (playerKickMode == BallKickMode.approachWithRun) {
            soccerPlayerAnimator.SetTrigger("GoalKick");
            ballKickPointPositionedRig.transform.position = ProjectPointOntoFloor(ballKickPointWithRunRelative.transform.position,0);
            transform.parent = ballKickPointPositionedRig.transform;
            playerKickMode = BallKickMode.animateWithRun;
            Debug.Log(gameObject.name + " performing goal kick with run");

        } else if (playerKickMode == BallKickMode.approachFast) {
            soccerPlayerAnimator.SetTrigger("GoalKickFast");
            ballKickPointPositionedRig.transform.position = ProjectPointOntoFloor(ballKickPointFastRelative.transform.position, 0);
            transform.parent = ballKickPointPositionedRig.transform;
            playerKickMode = BallKickMode.animateFast;
            Debug.Log(gameObject.name + " performing goal kick fast");
        }
    }

    void OnBallHit(IMessage rMessage)
    {
        Debug.Log((string)rMessage.Data + " hits the ball");
        if(goalKickForceTimeout > 0)
        {
            Debug.Log("Force timeout applied");
            return;
        }

        if (gameObject.name == (string)rMessage.Data)
        {
            Vector3 goalKickDirection = new Vector3();
            Vector3 tacticPointDirection = new Vector3();
            //Perform ball hit
            if(playerKickMode == BallKickMode.dribble) {
                tacticPointDirection = tacticPoint.transform.position - soccerWorld.soccerBall.transform.position;
                goalKickDirection = Vector3.Slerp(transform.forward, tacticPointDirection, dribbleKickDirectionLerp);
                soccerWorld.soccerBallRigidBody.AddForce(goalKickDirection.normalized / dribbleForceClamp, ForceMode.Acceleration);
                Debug.DrawRay(transform.position, transform.forward, Color.yellow, debugRayDuration);
                Debug.DrawRay(soccerWorld.soccerBall.transform.position, goalKickDirection.normalized, Color.red, debugRayDuration);
                Debug.Log("Applied force for dribble kick");

            } else {
                ApplyGoalKickForce();
            }

            Debug.DrawRay(soccerWorld.soccerBall.transform.position, goalKickDirection.normalized, Color.cyan, debugRayDuration);
        }
    }

    void ApplyGoalKickForce() {
        float goalKickPower = 0;
        Vector3 goalKickDirection = new Vector3();
        Vector3 tacticPointDirection = new Vector3();
        //Setup ball physics
        if (playerKickMode == BallKickMode.animateWithRun)
        {
            soccerWorld.soccerBallLogic.maxSpeed = goalKickWithRunMaxSpeed;
            soccerWorld.soccerBallLogic.forceClamp = goalKickWithRunForceClamp;
            goalKickPower = goalKickWithRunPower;
            Debug.Log("Preparing force for goal kick with run");

        }
        else if (playerKickMode == BallKickMode.animateFast)
        {
            soccerWorld.soccerBallLogic.maxSpeed = goalKickFastMaxSpeed;
            soccerWorld.soccerBallLogic.forceClamp = goalKickFastForceClamp;
            goalKickPower = goalKickFastPower;
            Debug.Log("Preparing force for fast goal kick");
        }
        goalKickDirection = kickDirectionWithRun.transform.position - soccerWorld.soccerBall.transform.position;
        //Stop correction
        playerKickMode = BallKickMode.dribble;
        //Kick the ball
        soccerWorld.soccerBallRigidBody.AddForce(goalKickDirection.normalized * goalKickPower, ForceMode.Impulse);
        goalKickForceTimeout = goalKickForceTimeoutLength;
        //Dissasemble corrective rig
        GameObject soccerKickPointTmp = transform.parent.gameObject;
        transform.parent = null;
        Destroy(soccerKickPointTmp);

        Debug.DrawRay(soccerWorld.soccerBall.transform.position, goalKickDirection.normalized, Color.cyan, debugRayDuration);
        Debug.Log("Applied force for goal kick");
    }

    void SetMoveTarget() {
        if (playerKickMode == BallKickMode.approachWithRun || playerKickMode == BallKickMode.approachFast) SetMoveTargetForGoalKick();
        else if (dribbleTarget)
        {
            Vector3 tacticPointVector = ProjectPointOntoFloor(dribbleTarget.transform.position, 0) - ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0);
            ballHitVector = Vector3.ClampMagnitude(tacticPointVector, ballTouchDistance) * -1;
            ballTouchPoint = soccerWorld.soccerBall.transform.position + ballHitVector;

            moveTarget.transform.position = ProjectPointOntoFloor(ballTouchPoint, 0.01F);

            Debug.DrawRay(ProjectPointOntoFloor(soccerWorld.soccerBall.transform.position, 0), ballHitVector, Color.grey);
        }
    }

    void SetMoveTargetForGoalKick() {
        Vector3 goalKickPointToBall = new Vector3();
        float kickPointToBallAngle = 0;
        float kickPointToBallDistance = 0;

        if (playerKickMode == BallKickMode.approachWithRun)
        {
            goalKickPointToBall = soccerWorld.soccerBall.transform.position - ballKickPointWithRunRelative.transform.position;
            kickPointToBallAngle = Vector3.SignedAngle(
            ballKickPointWithRunRelative.transform.position - transform.position,
            soccerWorld.soccerBall.transform.position - transform.position,
            Vector3.up
            );
            kickPointToBallDistance = Vector3.Distance(ballKickPointWithRunRelative.transform.position, soccerWorld.soccerBall.transform.position);
            if (debugUpdate)Debug.Log(gameObject.name + " setting up move target approaching with run");

        } else if (playerKickMode == BallKickMode.approachFast) {
            goalKickPointToBall = soccerWorld.soccerBall.transform.position - ballKickPointFastRelative.transform.position;
            kickPointToBallAngle = Vector3.SignedAngle(
            ballKickPointFastRelative.transform.position - transform.position,
            soccerWorld.soccerBall.transform.position - transform.position,
            Vector3.up
            );
            if (debugUpdate) Debug.Log(gameObject.name + " setting up move target approaching fast");
            kickPointToBallDistance = Vector3.Distance(ballKickPointFastRelative.transform.position, soccerWorld.soccerBall.transform.position);
        }

        moveTarget.transform.position = transform.position + goalKickPointToBall;
        if (Mathf.Abs(kickPointToBallAngle) < 5 && kickPointToBallDistance < 0.2) goalKickTargetLocked = true;

        if (debugUpdate) Debug.Log("Goal Kick Point to ball angle, distance " + kickPointToBallAngle + ", " + kickPointToBallDistance);
    }

    void OnGoalKickCLick(IMessage rMessage) {
        playerKickMode = BallKickMode.approachWithRun;
        Debug.Log(gameObject.name + " approaching for goal kick with run");
    }

    void OnGoalKickFastCLick(IMessage rMessage)
    {
        playerKickMode = BallKickMode.approachFast;
        Debug.Log(gameObject.name + " approaching for fast goal kick");
    }
}
