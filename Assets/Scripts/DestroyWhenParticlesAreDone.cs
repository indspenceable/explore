using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyWhenParticlesAreDone : MonoBehaviour {
	ParticleSystem system;
	// Use this for initialization
	void Start () {
		system = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!system.isPlaying) {
			Destroy(gameObject);
		}
	}
}
