using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerTakeDamage))]
public class PlayerUI : MonoBehaviour {
	private PlayerTakeDamage health;
//	public healthmeter healthMeter;

	public void Start() {
		health = GetComponent<PlayerTakeDamage>();
	}

	public void Update() {
//		playerHealthText.text = "Health: " + health.currentHealth;
//		healthMeter.
	}
}
