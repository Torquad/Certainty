using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum PlayerState
{
	Carrying,
	NotCarrying
}

public class RaycastShoot : MonoBehaviour {

	public int gunDamage = 1;											// Set the number of hitpoints that this gun will take away from shot objects with a health script
	//public float fireRate = 0.25f;										// Number in seconds which controls how often the player can fire
	public float weaponRange = 50f;										// Distance in Unity units over which the player can fire
	public float hitForce = 100f;										// Amount of force which will be added to objects with a rigidbody shot by the player
	public Transform gunEnd;											// Holds a reference to the gun end object, marking the muzzle location of the gun
	public Text interactText;
	Vector3 rayOrigin;

	//RigidbodyConstraints freezeAxes = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	float interactRange = 3f;
	private Camera fpsCam;												// Holds a reference to the first person camera
	private ConfigurableJoint joint;
	public Rigidbody anchor;
	private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);	// WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
	private AudioSource audioSource;										// Reference to the audio source which will play our shooting sound effect
	private ParticleSystem laserParticles;

    public AudioClip gunSound;
    public AudioClip warpSound;
	public AudioClip interactSound;
	public AudioClip returnSound;
    private LineRenderer laserLine;										// Reference to the LineRenderer component which will display our laserline
	private float nextFire;												// Float to store the time the player will be allowed to fire again, after firing
	PlayerState state = PlayerState.NotCarrying;
	public GameObject particleObject;

	GameObject moveObject;
	MovableBox move;
	ReturnPad moveRp;

	Rigidbody moveRb;
	Rigidbody lastRb;

	Transform fpsController;

	public Material laserMat;

	void Start () 
	{

		// Get and store a reference to our LineRenderer component
		laserLine = GetComponent<LineRenderer>();

		// Get and store a reference to our AudioSource component
		audioSource = GetComponent<AudioSource>();

		laserParticles = particleObject.GetComponent<ParticleSystem> ();

        // Get and store a reference to our Camera by searching this GameObject and its parents
        fpsCam = GetComponentInParent<Camera>();
		fpsController = fpsCam.transform.parent;
		laserLine.material = laserMat;
	}

	void Update () 
	{
		// Create a vector at the center of our camera's viewport
		rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

		CheckInteract ();

		if (state == PlayerState.NotCarrying)
			CheckShoot ();
	}

	private void CheckInteract()
	{
		// Declare a raycast hit to store information about what our raycast has hit
		RaycastHit hit;

		switch(state)
		{
			case PlayerState.Carrying:
				//disable laser line
				laserLine.enabled = false;
				if (Input.GetKeyUp ("e"))
				{
					state = PlayerState.NotCarrying;

					//drop
					audioSource.clip = interactSound;
					audioSource.Play ();


					interactText.text = "[E] Pick Up";

					//move.IsPickedUp = false;

					moveRb.useGravity = true;
					moveRb.freezeRotation = false;
					Destroy (joint);
					moveRb.transform.parent = null;
					moveRb = null;
				}
				break;

			case PlayerState.NotCarrying:
				//is interactable object in range?
				if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, interactRange))
				{
					moveObject = hit.collider.gameObject;

					moveRp = moveObject.GetComponent<ReturnPad> ();
					move = moveObject.GetComponent<MovableBox> ();
					moveRb = moveObject.GetComponent<Rigidbody> ();

					if (moveRp != null) // if you're looking at the return pad
					{
						interactText.text = "[E] Use return pad";
						interactText.enabled = true;

						if (Input.GetKeyUp ("e"))
						{
							// play some sound
							//audioSource.clip = interactSound;
							//audioSource.Play ();

							moveRp.Return ();
						}
					}
					else if (move != null) //if box is movable
					{
						interactText.text = "[E] Pick Up";
						interactText.enabled = true;


						if (Input.GetKeyUp ("e"))
						{
							state = PlayerState.Carrying;

							AddJoint ();
							// pick up
							audioSource.clip = interactSound;
							audioSource.Play ();

							interactText.text = "[E] Drop";

							moveRb.useGravity = false;
							moveRb.freezeRotation = true;


							moveRb.transform.SetParent (fpsCam.transform);
						}
					} 
					else
					{
						interactText.enabled = false;
					}
				}
				else
				{
					interactText.enabled = false;
				}
				break;
		}
	}

	private void CheckShoot()
	{
		// Declare a raycast hit to store information about what our raycast has hit
		RaycastHit hit;

		// Check if the player has pressed the warp button
		if (Input.GetButtonDown("Fire2"))
		{
			//////////////////PLAY AUDIO//////////////////
			audioSource.clip = warpSound;
			audioSource.Play();

			//////////////////USE RAYCAST//////////////////
			// Check if our raycast has hit anything
			if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
			{
				move = hit.collider.GetComponent<MovableBox>();

				if (move != null)
				{
					//Wait?
					move.Warp();
				}
			}
		}
		else if (Input.GetButton("Fire1"))
		{
			//////////////////PLAY AUDIO//////////////////
			audioSource.clip = gunSound;

			// Play the shooting sound effect
			if(!audioSource.isPlaying)
			{
				audioSource.Play();
			}

			//////////////////USE RAYCAST//////////////////
			// Turn on our line renderer
			laserLine.enabled = true;

			// Set the start position for our visual effect for our laser to the position of gunEnd
			laserLine.SetPosition (0, gunEnd.position);
			laserParticles.Play ();

			// Check if our raycast has hit anything
			if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
			{
				// Set the end position for our laser line 
				laserLine.SetPosition (1, hit.point);

				//laserLine.SetPosition(0, hit.point);
				//laserLine.SetPosition(1, hit.point + hit.normal);


				move = hit.collider.GetComponent<MovableBox> ();

				// Check if the object we hit is movable
				if (move == null)
					return;

				//all movable objects have rigidbodies
				hit.rigidbody.freezeRotation = true;

				//hitting a different object
				if (hit.rigidbody != lastRb)
				{
					UnfreezeLastObject ();
					lastRb = hit.rigidbody;
				}

				// Add force to the rigidbody we hit, in the direction from which it was hit
				hit.rigidbody.AddForce(-hit.normal * hitForce);
			}
			else
			{
				// If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
				UnfreezeLastObject ();
				laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
			}
		}
		else //neither button pressed, end beam
		{
			UnfreezeLastObject ();
			laserLine.enabled = false;
			if (audioSource.clip == gunSound)
			{
				audioSource.Stop ();
			}
			laserParticles.Stop ();
			laserParticles.Clear ();
		}
	}

	private void UnfreezeLastObject()
	{
		if (lastRb != null)
		{
			lastRb.freezeRotation = false;
			lastRb = null;
		}
	}
	private void AddJoint ()
	{
		joint = moveObject.AddComponent<ConfigurableJoint> ();

		joint.connectedBody = anchor;
		joint.autoConfigureConnectedAnchor = false;
		joint.anchor = new Vector3 (0f, 0f, -0.5f);
		joint.targetPosition = anchor.position;
		joint.targetRotation = anchor.rotation;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.connectedAnchor = Vector3.zero;
		//joint.linearLimitSpring = 50;
		//joint.
		//joint.rotationDriveMode = RotationDriveMode.Slerp;
		//joint.
		//JointDrive drive = new JointDrive ();
		//drive.maximumForce = 100f;
		//joint.xDrive = drive;

		//joint.angularXDrive.
		//joint.linearLimit.bounciness = 0;
		//joint.linearLimit.

		//moveRb.MovePosition (anchor.position);
		//moveRb.MoveRotation (anchor.rotation);
		moveRb.transform.position = anchor.position;
		moveRb.transform.rotation = anchor.rotation;
	}
}