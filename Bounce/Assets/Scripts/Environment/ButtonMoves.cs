using UnityEngine;
using System.Collections;

/// <summary>
/// Used for a button which will move another scene object when it is pressed.
/// NOTE: use this for a parent-child prefab system..
/// </summary>
public class ButtonMoves : MonoBehaviour {

	public Transform moveObj;
	public Transform buttonObj;
	public float xOffset;
	public float yOffset;
	public float moveTime = 5.0f;
	public float pressTime = 0.5f;
	public Transform[] otherButtons;		//special case if multiple buttons need to be pressed at once

	private bool depressed = false;	//whether player is on button
	private bool activated = false;	//whether the button event happened
	private Vector3 originalScale;
	private Vector3 pressOffset;
	private float pressTimer = 0.0f;
	private Transform pressPerson;

	// Use this for initialization
	void Start () {
		originalScale = buttonObj.localScale;
		float height = buttonObj.GetComponent<SpriteRenderer>().sprite.bounds.size.y;	//?
		pressOffset = new Vector3 (0f, -height / 2, 0f);
		//heightDiff = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (depressed && !activated)
		{
			pressTimer += Time.deltaTime;
			if (pressTimer >= pressTime)
			{
				if (otherButtons.Length > 0)
				{
					bool flag = true;
					for (int i = 0; i < otherButtons.Length; i++)
					{
						ButtonMoves script = otherButtons[i].GetComponent<ButtonMoves>();
						if (script.pressTimer < script.pressTime)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						for (int i = 0; i < otherButtons.Length; i++)
						{
							ButtonMoves script = otherButtons[i].GetComponent<ButtonMoves>();
							script.activated = true;
						}
						activated = true;
						moveObj.GetComponent<AnimatedMover>().MoveRelative(
							new Vector3(xOffset, yOffset, 0f), moveTime);
					}
				}
				else
				{
					activated = true;
					moveObj.GetComponent<AnimatedMover>().MoveRelative(
						new Vector3(xOffset, yOffset, 0f), moveTime);
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player" || col.tag == "MotherFollower")
		{
			if (!depressed)
			{
				buttonObj.localScale = new Vector3(originalScale.x,
				                                   originalScale.y / 2,
				                                   originalScale.z);
				buttonObj.position += pressOffset;
			}
			depressed = true;
			pressPerson = col.transform;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if ((col.tag == "Player" || col.tag == "MotherFollower")
		    && pressPerson != null && pressPerson == col.transform)
		{
			if (depressed)
			{
				buttonObj.localScale = originalScale;
				buttonObj.position -= pressOffset;
			}
			depressed = false;
			if (!activated)
				pressTimer = 0f;	//reset timer
		}
	}
}
