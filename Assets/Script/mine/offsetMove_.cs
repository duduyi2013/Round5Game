using UnityEngine;
using System.Collections;

public class offsetMove_ : MonoBehaviour {


	public GameObject _camera;
	private Vector3 cameraPosition;
	private Vector3 cameralastFramePosition;
	private Vector3 posDelt;
	public Rigidbody _rigidbody;
	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		cameraPosition = _camera.transform.localPosition;
		//transform.position =  
		posDelt = cameraPosition - cameralastFramePosition;
		this._rigidbody.velocity = posDelt / Time.deltaTime;
		cameralastFramePosition = _camera.transform.localPosition;

	}
}
