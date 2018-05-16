using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBallProjection : MonoBehaviour {
    public GameObject ball;
    public int rotationSpeed = 10;

	// Use this for initialization
	void Start () {
        ball = GameObject.FindGameObjectWithTag("Ball");	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(ball.transform.position.x, 0.01F, ball.transform.position.z);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("Dribble Left");
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Dribble Right");
            transform.Rotate(Vector3.up * Time.deltaTime * -rotationSpeed, Space.World);
        }
    }
}
