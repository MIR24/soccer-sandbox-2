using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour {

    public GameObject tacticTarget;
    public GameObject tacticPoint;

	// Use this for initialization
	void Start () {
        tacticTarget = GameObject.FindGameObjectWithTag("Ball");
        Debug.Log(new Vector3(2,2,2) - new Vector3(3,3,3));
	}
	
	// Update is called once per frame
	void Update () {
        if (tacticTarget) {
            Vector3 tacticTargetVector = projectPointOntoFloor(transform.position) - projectPointOntoFloor(tacticTarget.transform.position);
            Debug.DrawRay(projectPointOntoFloor(tacticTarget.transform.position), tacticTargetVector, Color.green);

            if (tacticPoint){
                Vector3 tacticPointVector = projectPointOntoFloor(tacticPoint.transform.position) - projectPointOntoFloor(tacticTarget.transform.position);
                Debug.DrawRay(projectPointOntoFloor(tacticTarget.transform.position), tacticPointVector.normalized, Color.yellow);
            }
        }




    }

    Vector3 projectPointOntoFloor(Vector3 pointToProject) {
        Vector3 projectedPoint = new Vector3(pointToProject.x, 0, pointToProject.z);
        
        return projectedPoint;
    }
}
