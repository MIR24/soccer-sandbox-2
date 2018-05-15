using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour {

    public GameObject tacticTarget;
    public GameObject tacticPoint;

	// Use this for initialization
	void Start () {
        tacticTarget = GameObject.FindGameObjectWithTag("Ball");
	}
	
	// Update is called once per frame
	void Update () {
        if(tacticTarget) Debug.DrawLine(projectPointOntoFloor(transform.position), projectPointOntoFloor(tacticTarget.transform.position), Color.yellow);
		
	}

    Vector3 projectPointOntoFloor(Vector3 pointToProject) {
        Vector3 projectedPoint = new Vector3(pointToProject.x, 0, pointToProject.z);
        
        return projectedPoint;
    }
}
