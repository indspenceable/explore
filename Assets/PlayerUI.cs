using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerTakeDamage))]
public class PlayerUI : MonoBehaviour {
	private PlayerTakeDamage health;
	public Text playerHealthText;

	public void Start() {
		health = GetComponent<PlayerTakeDamage>();
	}

	public void Update() {
		playerHealthText.text = "Health: " + health.currentHealth;
	}
}
