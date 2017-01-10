using UnityEngine;
using System.Collections;

public class ReturnPad : MonoBehaviour {
	public Transform linkedCube;
	public Transform anchorTransform;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void Return()
	{
		linkedCube.position = anchorTransform.position;
		linkedCube.rotation = anchorTransform.rotation;
		linkedCube.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}
}
