using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	private GameObject screenFadeObj;
	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnTriggerEnter2D (Collider2D col){
		if (col.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			Death.respawn = this.gameObject.transform.position;
			Death.camConfig = GameObject.FindGameObjectWithTag("MainCamera").
				GetComponent<CameraFollow>().GetConfig();
		}
	}
}
