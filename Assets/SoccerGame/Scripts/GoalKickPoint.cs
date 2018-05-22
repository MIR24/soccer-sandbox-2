using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKickPoint : MonoBehaviour {

    SoccerPlayer entityComponent;

	void Start () {
        entityComponent = gameObject.transform.root.gameObject.GetComponent<SoccerPlayer>();
    }
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Ball") {
            Debug.Log(gameObject.transform.root.gameObject.name + " can perform goal kick right now");
            //entityComponent.goalKickLocked = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            Debug.Log(gameObject.transform.root.gameObject.name + " lost goal kick opportunity");
            //entityComponent.goalKickLocked = false;
        }
    }
}
