using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour, IInteractable {
	public Transform destination;

	public void Interact() {
		GameManager gm = GameManager.instance;
		gm.StartCoroutine(gm.DoRoomTransitionFull(destination.position));
	}
}
