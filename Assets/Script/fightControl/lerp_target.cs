using UnityEngine;
using System.Collections;

public class lerp_target : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = this.transform.parent.transform.position;
	}
}
