using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTransform1 : MonoBehaviour {

    public GameObject HeadTrack;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = HeadTrack.transform.position;
    }
}
