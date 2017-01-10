using UnityEngine;
using System.Collections;

public class PlayerEnterTrigger : MonoBehaviour {
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

	void OnTriggerEnter (Collider other)
	{
		if(other.tag == "Player" && triggerEnabled)
		{
			script.Invoke (functionToCall,0f);
			triggerEnabled = false;
		}
	}
}
