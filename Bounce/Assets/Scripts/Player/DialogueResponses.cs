using UnityEngine;
using System.Collections.Generic;

public class DialogueResponses : MonoBehaviour {

	public Transform textMeshPrefab;
	public int cursor;

	private List<Transform> textList 
		= new List<Transform>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetResponses(List<string> responses) 
	{
		if (responses == null || responses.Count == 0)
		{
			DestroyResponses();
			return;
		}

		cursor = 0;
		int i = 0;
		DestroyResponses();
		foreach (string response in responses)
		{
			Transform obj = Instantiate(textMeshPrefab, transform.position, Quaternion.identity) as Transform;
			obj.GetComponent<TextMesh>().text = response;
			obj.transform.position = new Vector3(transform.position.x, 
			                                     transform.position.y + 1.5f - i*0.4f,
			                                     transform.position.z);
			textList.Add(obj);
			i++;
		}
		if (responses.Count > 1)
			textList[0].GetComponent<TextMesh>().color = Color.yellow;	//current selection
	}

	public void DestroyResponses()
	{
		foreach (Transform trans in textList)
			Destroy (trans.gameObject);
		textList.Clear();
	}

	public void MoveCursor(bool up)
	{
		int numChoices = textList.Count;
		if (numChoices <= 1)
			return;
		textList[cursor].GetComponent<TextMesh>().color = Color.black;
		if (up)
		{
			cursor--;
			if (cursor < 0)
				cursor = numChoices - 1;
		}
		else //otherwise it's down!
		{
			cursor = (cursor + 1) % numChoices;
		}
		textList[cursor].GetComponent<TextMesh>().color = Color.yellow;
	}
}
