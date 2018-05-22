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
    public float goalKickPower=2F;

    private Rigidbody myRigidBody;

    void Start () {
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        goalKickPoint = GameObject.Find("GoalKickDirection");
        myRigidBody = gameObject.GetComponent<Rigidbody>();
    }
	
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
        }
    }

    private void FixedUpdate()
    {
        if (myRigidBody.velocity.magnitude > maxSpeed)
        {
            myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, maxSpeed);
        }
    }
}
