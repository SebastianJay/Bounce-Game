using UnityEngine;
using System.Collections.Generic;

public class PlayerSoundManager : MonoBehaviour {

	public AudioClip jumpSound;
	public AudioClip bounceSound;
	public AudioClip superJumpSound;
	public AudioClip spiderJumpSound;
	public AudioClip spiderStickSound;
	public AudioClip balloonPopSound;
	public AudioClip balloonSnapSound;

	public float jumpVolume = 1f;
	public float bounceVolume = 1f;
	public float superJumpVolume = 1f;
	public float spiderJumpVolume = 1f;
	public float spiderStickVolume = 1f;
	public float balloonPopVolume = 1f;
	public float balloonSnapVolume = 1f;

	private Dictionary<string, AudioSource> mappings;
	private GameObject screenFadeObj;
	// Add all the new audio sources on initialization
	// Be aware of the strings used as keys in map initialization
	// 	these keys will be used elsewhere when PlaySound is called
	void Awake () {
		mappings = new Dictionary<string, AudioSource>();
		if (jumpSound != null){
			AudioSource jumpSrc = gameObject.AddComponent<AudioSource>();
			jumpSrc.clip = jumpSound;
			jumpSrc.volume = jumpVolume;
			mappings.Add("Jump", jumpSrc);
		}
		if (bounceSound != null){
			AudioSource bounceSrc = gameObject.AddComponent<AudioSource>();
			bounceSrc.clip = bounceSound;
			bounceSrc.volume = bounceVolume;
			mappings.Add("Bounce", bounceSrc);
		}
		if (superJumpSound != null){
			AudioSource sjumpSrc = gameObject.AddComponent<AudioSource>();
			sjumpSrc.clip = superJumpSound;
			sjumpSrc.volume = superJumpVolume;
			mappings.Add("SuperJump", sjumpSrc);
		}
		if (spiderJumpSound != null){
			AudioSource sjumpSrc = gameObject.AddComponent<AudioSource>();
			sjumpSrc.clip = spiderJumpSound;
			sjumpSrc.volume = spiderJumpVolume;
			mappings.Add("SpiderJump", sjumpSrc);
		}
		if (spiderStickSound != null){
			AudioSource sStickSrc = gameObject.AddComponent<AudioSource>();
			sStickSrc.clip = spiderStickSound;
			sStickSrc.volume = spiderStickVolume;
			mappings.Add("SpiderStick", sStickSrc);
		}
		if (balloonPopSound != null) {
			AudioSource bPopSrc = gameObject.AddComponent<AudioSource>();
			bPopSrc.clip = balloonPopSound;
			bPopSrc.volume = balloonPopVolume;
			mappings.Add("BalloonPop", bPopSrc);
		}
		if (balloonSnapSound != null) {
			AudioSource bSnapSrc = gameObject.AddComponent<AudioSource>();
			bSnapSrc.clip = balloonSnapSound;
			bSnapSrc.volume = balloonSnapVolume;
			mappings.Add("BalloonSnap", bSnapSrc);
		}
		screenFadeObj = GameObject.FindGameObjectWithTag ("ScreenFader");
	}

	public void PlaySound(string key)
	{
		if (mappings.ContainsKey (key)
		    && (screenFadeObj==null || (screenFadeObj!=null && !screenFadeObj.GetComponent<ScreenFading>().IsTransitioning())))
			mappings[key].Play ();
		//else
		//	Debug.LogWarning("PlayerSoundManager could not find sound corresponding to " + key);
	}
}
