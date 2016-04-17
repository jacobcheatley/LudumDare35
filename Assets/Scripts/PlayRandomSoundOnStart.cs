using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSoundOnStart : MonoBehaviour
{
    [SerializeField] private List<AudioClip> sounds;

	void Start()
	{
	    GetComponent<AudioSource>().PlayOneShot(sounds[UnityEngine.Random.Range(0, sounds.Count)]);
	}
}
