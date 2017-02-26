using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoCredits : MonoBehaviour, IActivatableObject {
	public GameObject credits;
	public void Activate(Level l) {
		l.gameObject.SetActive(false);
		GameManager.instance.player.GetComponent<PlayerTakeDamage>().meter.gameObject.SetActive(false);
		GameManager.instance.player.gameObject.SetActive(false);
		credits.SetActive(true);
		GameManager.instance.GameIsActiveState = 2;
	}
}
