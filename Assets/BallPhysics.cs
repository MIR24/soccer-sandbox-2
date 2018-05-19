using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour {
    public GameObject tacticPoint;
    public float forceClamp = 1.2F;

    // Use this for initialization
    void Start () {
        tacticPoint = GameObject.FindGameObjectWithTag("TacticPoint");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.transform.root.gameObject.tag);
        if(collision.gameObject.transform.root.gameObject.tag == "Player")
            gameObject.GetComponent<Rigidbody>().AddForce((tacticPoint.transform.position-transform.position).normalized/forceClamp, ForceMode.Acceleration);
    }


}
