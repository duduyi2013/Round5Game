using UnityEngine;
using System.Collections;

public class Controller_possition_tracking : MonoBehaviour {
    public GameObject right_Controller;
    public Vector3 velocity;
    Vector3 _lastFramePosition;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = right_Controller.transform.position;
	}
    void FixedUpdate()
    {
        velocity = (this.transform.position - _lastFramePosition) / Time.fixedDeltaTime;
        _lastFramePosition = this.transform.position;
    }
}
