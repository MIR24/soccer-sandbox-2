using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class BallPhysics : MonoBehaviour {
    public GameObject tacticPoint;
    public GameObject goalKickPoint;
    public float forceClamp = 1.2F;
    public float maxSpeed = 1f;
    public float goalKickMaxSpeed = 50f;
    //public string performingGoalKick;
    public float goalKickPower=2F;

    private Rigidbody myRigidBody;

    // Use this for initialization
    void Start () {
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        goalKickPoint = GameObject.Find("GoalKickDirection");
        myRigidBody = gameObject.GetComponent<Rigidbody>();
        //MessageDispatcher.AddListener("GOAL_KICK", OnGoalKick, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball collided with (root tag, root name, object name):" 
            + collision.gameObject.transform.root.gameObject.tag + ", " 
            + collision.gameObject.transform.root.gameObject.name + ", "
            + collision.gameObject.name);
        if (collision.gameObject.transform.root.gameObject.tag == "Player")
        {
            MessageDispatcher.SendMessage(this, "BALL_HIT", collision.gameObject.transform.root.gameObject.name, 0);

            //if (collision.gameObject.transform.root.gameObject.name == performingGoalKick)
            //{
            //    Vector3 goalKickDirection = goalKickPoint.transform.position - transform.position;
            //    Debug.DrawRay(transform.position, goalKickDirection, Color.yellow, 3F);
            //    myRigidBody.AddForce(goalKickDirection.normalized * goalKickPower, ForceMode.Impulse);
            //    maxSpeed = goalKickMaxSpeed;
            //    Debug.Log("Applied force for goal kick");
            //}
            //else
            //{
            //    Vector3 goalKickDirection = tacticPoint.transform.position - transform.position;
            //    myRigidBody.AddForce(goalKickDirection.normalized / forceClamp, ForceMode.Acceleration);
            //}
        }
    }

    //void OnGoalKick(IMessage rMessage)
    //{
    //    Debug.Log((string)rMessage.Data + " performing goal kick");
    //    if (performingGoalKick != null)
    //    {
    //        performingGoalKick = (string)rMessage.Data;
    //    }
    //}

    private void FixedUpdate()
    {
        if (myRigidBody.velocity.magnitude > maxSpeed)
        {
            myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, maxSpeed);
        }
    }
}
