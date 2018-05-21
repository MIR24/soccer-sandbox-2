using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class BallPhysics : MonoBehaviour {
    public GameObject tacticPoint;
    public float forceClamp = 1.2F;
    public float maxSpeed = 1f;

    private Rigidbody myRigidBody;

    // Use this for initialization
    void Start () {
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
        myRigidBody = gameObject.GetComponent<Rigidbody>();
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
            myRigidBody.AddForce((tacticPoint.transform.position - transform.position).normalized / forceClamp, ForceMode.Acceleration);
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
