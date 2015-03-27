using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour {

	public ItemType type;
	public AudioClip pickUpNoise;
	public float pickUpVolume = 1f;
	public bool unique = true;	//if true, item only spawns if player doesn't have it

	private GameObject screenFadeObj;
	private GameObject notifyObj;
	void Start()
	{
		if (unique && PlayerDataManager.inventory.HasItem (type))
			Destroy (gameObject);
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
		notifyObj = GameObject.FindGameObjectWithTag ("NoteManager");
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

			PlayerDataManager.inventory.AddItem(type);
			if (notifyObj != null) {
				notifyObj.GetComponent<NotificationManager>().PushMessage(
					"Added the item \"" + ImmutableData.GetItemData()[type].name + "\" to Inventory");
			}
			Destroy (this.gameObject);
		}
	}
}
