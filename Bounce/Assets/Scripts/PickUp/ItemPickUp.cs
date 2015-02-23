using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour {

	public ItemType type;
	public AudioClip pickUpNoise;
	public float pickUpVolume = 1f;

	private GameObject screenFadeObj;
	void Start()
	{
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player"
		    && (screenFadeObj == null || !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning()))
		{
			if (pickUpNoise != null) {
				//we create a dummy object for the noise since this one will get killed
				GameObject obj = new GameObject();
				obj.transform.position = this.transform.position;
				AudioSource src = obj.AddComponent<AudioSource>();
				src.clip = pickUpNoise;
				src.volume = pickUpVolume;
				obj.AddComponent<SelfRemove>();	//default 10 s
				src.Play();
			}
			//PlayerDataManager data = col.gameObject.GetComponent<PlayerDataManager>();
			//data.inventory.AddItem(type);
			PlayerDataManager.inventory.AddItem(type);
			Destroy (this.gameObject);
		}
	}
}
