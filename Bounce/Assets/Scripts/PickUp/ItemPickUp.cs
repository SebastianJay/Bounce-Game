using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour {
	
	public ItemType type;

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player")
		{
			Destroy (this.gameObject);
			PlayerDataManager data = col.gameObject.GetComponent<PlayerDataManager>();
			data.inventory.AddItem(type);
		}
	}
}
