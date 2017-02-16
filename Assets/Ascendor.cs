using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascendor : MonoBehaviour {
	public bool started = false;

	private BoxCollider2D coll;
	public LayerMask playerLayer;
	// Use this for initialization
	void Start () {
		this.coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!started && coll.IsTouchingLayers(playerLayer)) {
			started = true;
			StartCoroutine(Ascend());
		}
	}

	private IEnumerator Ascend() {
		while (true) {
			transform.Translate(Vector2.up * GameManager.instance.ActiveGameDeltaTime);
			yield return null;
		}
	}
}
