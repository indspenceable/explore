using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascendor : MonoBehaviour {
	public bool started = false;

	private BoxCollider2D coll;
	public LayerMask playerLayer;
	// Use this for initialization

	public float speedUp;
	public float speedRight;
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
			transform.Translate((Vector2.up*speedUp + Vector2.right * speedRight) * GameManager.instance.ActiveGameDeltaTime);
			yield return null;
		}
	}
}
