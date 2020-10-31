using UnityEngine;
using System.Collections;

public class GarageDoor : MonoBehaviour {
	public Transform	MessageUI;

	Animation doorAnim;
	AudioSource doorSounds;

	// Use this for initialization
	void Start () {
        doorAnim = GetComponent<Animation>();
		doorSounds = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame

	public void TriggerOpen()
    {
		doorAnim.Play("Open");
		doorSounds.Play();
	}

	public void TriggerClose()
	{
		doorAnim.Play("Close");
		doorSounds.Play();
	}
}
