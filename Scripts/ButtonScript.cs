using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {
	public AudioSource buttonDown;
	public Material materialOn;
	public Material materialOff;
	public GameObject door;

	private Renderer r;
	private Animation anim;

	// Use this for initialization
	void Awake () {
		r = this.gameObject.GetComponent<Renderer> ();
		r.material = materialOff;

		anim = door.GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		buttonDown.Play ();
		r.material = materialOn;
		anim.Play ("open");
	}
	void OnTriggerExit(Collider other)
	{
		buttonDown.Play ();
		r.material = materialOff;
		anim.Play ("close");
	}
}
