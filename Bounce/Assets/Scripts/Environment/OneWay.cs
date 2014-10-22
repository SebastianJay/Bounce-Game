using UnityEngine;
using System.Collections;

public class OneWay : MonoBehaviour {
	void Update () 
		//Has to check player velocity every update, else additional triggers would be necessary
	{
		if (GameObject.FindWithTag ("Player").rigidbody2D.velocity.y > 0) {
			//checks if player is going up
						this.gameObject.layer = 13;
			/*since the player is, the platform the script is attached to turns into a layer that only
			the player can go through*/
			} 
			else 
			//checks to see if player is stationary or going down
			{
			this.gameObject.layer = 0;
			/*ensures the platform is fully collidable once the player is not going up, setting the
			platform back to the default layer*/
			}
	}
}
