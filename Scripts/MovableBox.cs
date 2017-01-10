using UnityEngine;
using System.Collections;

public class MovableBox : MonoBehaviour {

	public AudioClip WarpIn;
	public AudioClip WarpOut;

	GameObject player;
	AudioSource warpSource;

	Vector3 direction;
    Rigidbody rb;
	MeshRenderer meshRend;
	MeshRenderer[] childRend;

	public ParticleSystem appear;
	public ParticleSystem disappear;


	int cellMask = 1 << 10; //cell layer


	bool isPickedUp;
	//public bool IsPickedUp {get; set;}

	void Awake () {
        //swap speed direction
		player = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody>();

		meshRend = GetComponent<MeshRenderer> ();
		childRend = GetComponentsInChildren<MeshRenderer> ();
		warpSource = GetComponent<AudioSource> ();
	}

	public void Warp()
	{
		StartCoroutine ("WarpCoroutine");
	}

	IEnumerator WarpCoroutine() {
		// Do initial animating, hide cube
		RaycastHit hit;

		direction = transform.forward;

		//swap height and speed
		float speed;
		speed = rb.velocity.magnitude;

		rb.constraints = RigidbodyConstraints.FreezeAll;

		float minHeight = 0;

		//find lower height bound at this position, prevent from going through floor or parts of the cell resting on the floor
		if (Physics.Raycast (rb.transform.position, Vector3.down, out hit, Mathf.Infinity, cellMask))
		{
			minHeight = hit.point.y + rb.transform.lossyScale.y/2; //place box adjacent to lower bound
		}
			
		//if (transform.position.y < minHeight)
		//	throw new UnityException ("Position below minimum, sqrt of negative");

		if (transform.position.y < minHeight) //if height bugged through floor
		{
			transform.position = new Vector3 (transform.position.x, minHeight, transform.position.z);
		}

		//v = sqrt(2*g*h)
		float newSpeed = Mathf.Sqrt(2f*Physics.gravity.magnitude*(transform.position.y - minHeight));


		//h = (v^2)/2g
		float maxHeight = Mathf.Infinity;
		float newHeight = Mathf.Pow(speed,2)/(2*Physics.gravity.magnitude) + minHeight;

		//find upper height bound at this position, prevent from going through ceiling
		if (Physics.Raycast (rb.transform.position, Vector3.up, out hit, Mathf.Infinity, cellMask))
		{
			maxHeight = hit.point.y - rb.transform.lossyScale.y/2; //place box adjacent to ceiling
		}

		newHeight = newHeight > maxHeight ? maxHeight : newHeight;

		Vector3 newPos = new Vector3(transform.position.x,newHeight,transform.position.z);

		disappear.Play();
		warpSource.clip = WarpIn;
		warpSource.Play ();
		meshRend.enabled = false;

		foreach (MeshRenderer child in childRend)
		{
			child.enabled = false;
		}

		////// WAIT & ANIMATE //////
		yield return new WaitForSeconds(0.5f);

		//Do final animation, update position, show cube

		////// SET NEW VALUES //////

		rb.constraints = RigidbodyConstraints.None;
		rb.velocity = direction * newSpeed;
		rb.position = newPos;

		appear.Play();

		warpSource.clip = WarpOut;
		warpSource.Play ();

		yield return new WaitForSeconds (0.1f);
		meshRend.enabled = true;
		foreach (MeshRenderer child in childRend)
		{
			child.enabled = true;
		}
    }
}