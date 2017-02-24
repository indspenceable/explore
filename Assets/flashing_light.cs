using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashing_light : MonoBehaviour {
	private SpriteRenderer sr;
	// Use this for initialization
	void Start () {
		this.sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time;
		t /= 10;
		t -= (int)t;
		this.sr.color = Color.HSVToRGB(t, 1f, 1f);
	}
}
