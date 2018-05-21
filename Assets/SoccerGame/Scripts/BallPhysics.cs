using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log(collision.gameObject.transform.root.gameObject.tag);
        if (collision.gameObject.transform.root.gameObject.tag == "Player")
            gameObject.GetComponent<Rigidbody>().AddForce((tacticPoint.transform.position - transform.position).normalized / forceClamp, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        if (myRigidBody.velocity.magnitude > maxSpeed)
        {
            myRigidBody.velocity = Vector3.ClampMagnitude(myRigidBody.velocity, maxSpeed);
        }
    }
}
