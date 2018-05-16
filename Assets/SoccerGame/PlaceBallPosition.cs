using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBallPosition : MonoBehaviour {
    public GameObject ball;

	// Use this for initialization
	void Start () {
        ball = GameObject.FindGameObjectWithTag("Ball");	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(ball.transform.position.x, 0.01F, ball.transform.position.z);
	}
}
