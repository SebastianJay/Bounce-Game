using UnityEngine;
using System.Collections;

public class ConstantSetOnHit : MonoBehaviour {

	public bool setConstant = true;
	public string constantName = "REPLACE_ME";
	public bool triggerEvent = false;
	public string eventName = "REPLACE_ME";
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			if (setConstant)
				DialogueConstantParser.SetConstant(constantName);
			if (triggerEvent)
				DialogueConstantParser.ExecuteEvent(eventName);
		}
	}
}
