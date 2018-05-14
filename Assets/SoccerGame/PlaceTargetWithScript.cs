using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTargetWithScript : MonoBehaviour {

    public GameObject physicalPositionTarget;
    public Vector3 targetOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(
            physicalPositionTarget.transform.position.x,
            transform.position.y,
            physicalPositionTarget.transform.position.z) + targetOffset;
	}
}
