using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public enum TutorialState
{
	Intro,
	IntroDiagram0,
	IntroDiagram1,
	IntroDiagram2,
	Done0,
	Puzzle2Diagram0,
	Puzzle2Diagram1,
	Puzzle2Diagram2,
	Done1
}

public class Tutorial : MonoBehaviour {
	public GameObject Intro_Image;
	public GameObject IntroDiagram0_Image;
	public GameObject IntroDiagram1_Image;
	public GameObject IntroDiagram2_Image;

	public GameObject Puzzle2Diagram0_Image;
	public GameObject Puzzle2Diagram1_Image;
	public GameObject Puzzle2Diagram2_Image;

	public GameObject leftButton;
	public GameObject rightButton;
	public GameObject doneButton;

	private FirstPersonController player;
	private RaycastShoot playerShoot;

	private TutorialState state;

	private bool hint;

	public TutorialState GetState()
	{
		return state;
	}

	// Use this for initialization
	void Start () {
		hint = false;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<FirstPersonController> (); // as FirstPersonController;
		playerShoot = player.GetComponentInChildren<RaycastShoot> ();
		BeginTutorial0();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonPress(bool left) //default argument for invoke() trigger call (does not matter what the value is)
	{
		TutorialState nextState;

		//determine next state
		switch (state)
		{
			case TutorialState.Intro:
				Intro_Image.SetActive (false);
				//leftButton.SetActive (true);
				nextState = TutorialState.IntroDiagram0;
				break;
			
			case TutorialState.IntroDiagram0:
				IntroDiagram0_Image.SetActive (false);
				nextState = left ? TutorialState.Intro : TutorialState.IntroDiagram1;
				break;
			case TutorialState.IntroDiagram1:
				IntroDiagram1_Image.SetActive (false);
				nextState = left ? TutorialState.IntroDiagram0 : TutorialState.IntroDiagram2;
				break;
			case TutorialState.IntroDiagram2:
				IntroDiagram2_Image.SetActive (false);
				//rightButton.SetActive (true);
				//doneButton.SetActive (false);
				nextState = left ? TutorialState.IntroDiagram1 : TutorialState.Done0;
				break;
//			case TutorialState.Done0: //should never happen
				//setup Puzzle2 Tutorial
//				BeginTutorial1();
//				nextState = TutorialState.Puzzle2Diagram0;
//				throw new UnityException("Done Button Press");
//				nextState = TutorialState.Intro;
//				break;
			case TutorialState.Puzzle2Diagram0:
				Puzzle2Diagram0_Image.SetActive (false);
				nextState = TutorialState.Puzzle2Diagram1;
				break;
			case TutorialState.Puzzle2Diagram1:
				Puzzle2Diagram1_Image.SetActive (false);
				nextState = left ? TutorialState.Puzzle2Diagram0 : TutorialState.Puzzle2Diagram2;
				break;
			case TutorialState.Puzzle2Diagram2:
				Puzzle2Diagram2_Image.SetActive (false);
				nextState = left ? TutorialState.Puzzle2Diagram1 : TutorialState.Done1;
				break;
			default:
				nextState = TutorialState.Intro;
				throw new UnityException ("Bad State");
		}

		//hide and show UI elements based on next state
		switch (nextState)
		{
			case TutorialState.Intro:
				Intro_Image.SetActive (true);

				leftButton.SetActive (false);
				rightButton.SetActive (true);
				doneButton.SetActive (false);
				break;

			case TutorialState.IntroDiagram0:
				IntroDiagram0_Image.SetActive (true);

				leftButton.SetActive (true);
				rightButton.SetActive (true);
				doneButton.SetActive (false);
				break;

			case TutorialState.IntroDiagram1:
				IntroDiagram1_Image.SetActive (true);

				leftButton.SetActive (true);
				rightButton.SetActive (true);
				doneButton.SetActive (false);
				break;

			case TutorialState.IntroDiagram2:
				IntroDiagram2_Image.SetActive (true);

				leftButton.SetActive (true);
				rightButton.SetActive (false);
				doneButton.SetActive (true);
				break;

			case TutorialState.Done0:
				IntroDiagram2_Image.SetActive (false);
				EndTutorial ();
				break;

			case TutorialState.Puzzle2Diagram0:
				Puzzle2Diagram0_Image.SetActive (true);

				leftButton.SetActive (false);
				rightButton.SetActive (true);
				doneButton.SetActive (false);
				break;

			case TutorialState.Puzzle2Diagram1:
				Puzzle2Diagram1_Image.SetActive (true);

				leftButton.SetActive (true);
				rightButton.SetActive (true);
				doneButton.SetActive (false);
				break;

			case TutorialState.Puzzle2Diagram2:
				Puzzle2Diagram2_Image.SetActive (true);

				leftButton.SetActive (true);
				rightButton.SetActive (false);
				doneButton.SetActive (true);
				break;

			case TutorialState.Done1:
				Puzzle2Diagram2_Image.SetActive (false);
				EndTutorial ();
				break;
		}

		state = nextState;
	}

	void BeginTutorial0()
	{
		Intro_Image.SetActive (true);
		SetupTutorial ();
		state = TutorialState.Intro;
	}	

	void BeginTutorial1()
	{
		Puzzle2Diagram0_Image.SetActive (true);
		SetupTutorial ();
		state = TutorialState.Puzzle2Diagram0;
	}

	void SetupTutorial()
	{
		leftButton.SetActive (false);
		rightButton.SetActive (true);
		doneButton.SetActive (false);

		player.enabled = false;
		playerShoot.enabled = false;
		player.m_MouseLook.SetCursorLock (false);
	}

	void EndTutorial()
	{
		leftButton.SetActive (false);
		rightButton.SetActive (false);
		doneButton.SetActive (false);
		if (!hint)
		{
			player.enabled = true;
			playerShoot.enabled = true;
			player.m_MouseLook.SetCursorLock (true);
		}

		hint = false;
	}

	public void Hint()
	{
		hint = true;
		if (state == TutorialState.Done0)
			//show first tutorial
			BeginTutorial0 ();
		else if (state == TutorialState.Done1)
			BeginTutorial1 ();
		
			//otherwise you're currently in a tutorial, do nothing
	}
}
