using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerWorld : MonoBehaviour {

    public float timeScale = 1F;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(timeScale != Time.timeScale) Time.timeScale = timeScale;
    }
}
