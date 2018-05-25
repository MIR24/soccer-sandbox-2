using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Messages;

public class SoccerWorld : MonoBehaviour {

    public float timeScale = 1F;
    public GameObject soccerBall;
    public Rigidbody soccerBallRigidBody;
    public BallPhysics soccerBallLogic;

	// Use this for initialization
	void Start () {
        soccerBall = GameObject.FindGameObjectWithTag("Ball");
        soccerBallRigidBody = soccerBall.GetComponent<Rigidbody>();
        soccerBallLogic = soccerBall.GetComponent<BallPhysics>();
	}
	
	// Update is called once per frame
	void Update () {
        if(timeScale != Time.timeScale) Time.timeScale = timeScale;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 75, 30), "GoalKick")) {
            MessageDispatcher.SendMessage(this, "GOAL_KICK_CLICK", "Dummy Data", 0);
            Debug.Log("Clicked the Goal Kick button");
        }
        if (GUI.Button(new Rect(10, 50, 75, 30), "Pass"))
        {
            MessageDispatcher.SendMessage(this, "GOAL_KICK_FAST_CLICK", "Dummy Data", 0);
            Debug.Log("Clicked the Goal Kick Fast button");
        }

        if (GUI.Button(new Rect(10, 90, 75, 30), "TL /2"))
        {
            timeScale = timeScale/2;
        }

        if (GUI.Button(new Rect(10, 130, 75, 30), "TL *2"))
        {
            timeScale = timeScale*2;
        }

        if (GUI.Button(new Rect(10, 170, 75, 30), "TL 1"))
        {
            timeScale = 1F;
        }

        GUI.Label(new Rect(95, 10, 100, 20), timeScale.ToString());
    }

}
