using UnityEngine;
using System.Collections;

public class PlayerEnterTrigger : MonoBehaviour {
	//as general as possible so I can reuse
	public MonoBehaviour script;
	public string functionToCall;

	bool triggerEnabled;

	// Use this for initialization
	void Start () {
		triggerEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider other) //call any function with 0 arguments on any script OnTriggerEnter
	{
		if(other.tag == "Player" && triggerEnabled)
		{
			script.Invoke (functionToCall,0f);
			triggerEnabled = false;
		}
	}
}
