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
	public AudioClip sound;

	void Start () {
		this.coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!started && coll.IsTouchingLayers(playerLayer)) {
			started = true;
			StartCoroutine(Ascend());
			StartCoroutine(PlaySound());
		}
	}

	private IEnumerator Ascend() {
		while (true) {
			transform.Translate((Vector2.up*speedUp + Vector2.right * speedRight) * GameManager.instance.ActiveGameDeltaTime);
			yield return null;
		}
	}

	private IEnumerator PlaySound() {
		while (true) {
			GameManager.instance.PlaySound(sound);
			float dt = 0;
			while (dt < 1f) {
				yield return null;
				dt += GameManager.instance.ActiveGameDeltaTime;
			}
		}
	}
}
