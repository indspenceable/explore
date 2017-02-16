using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savepoint : MonoBehaviour {
	public bool currentlyTouchingPlayer = false;
	public Collider2D col;
	public LayerMask playerLayers;
	public AudioClip saveSoundEffect;
	public void Start() {
		col = GetComponent<Collider2D>();
	}
	public void Update() {
		bool newTouching = col.IsTouchingLayers(playerLayers);
		if (newTouching != currentlyTouchingPlayer) {
			if ( newTouching ) {
				GameManager.instance.player.gameObject.GetComponent<PlayerTakeDamage>().Start();
				GameManager.instance.SaveGameState(0, transform.position);
				GameManager.instance.PlaySound(saveSoundEffect);
			}
			currentlyTouchingPlayer = newTouching;
		}
	}
}
