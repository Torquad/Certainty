using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {
	public AudioSource buttonDown;
	public Material materialOn;
	public Material materialOff;
	public GameObject door;

	// Use this for initialization
	void Awake () {
		Renderer r = this.gameObject.GetComponent<Renderer> ();
		r.material = materialOff;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		buttonDown.Play ();
		Renderer r = this.gameObject.GetComponent<Renderer> ();
		r.material = materialOn;
		Animation yolo = door.GetComponent<Animation> ();
		yolo.Play ("open");
	}
	void OnTriggerExit(Collider other)
	{
		buttonDown.Play ();
		Renderer r = this.gameObject.GetComponent<Renderer> ();
		r.material = materialOff;
		door.GetComponent<Animation>().Play ("close");
	}
}
