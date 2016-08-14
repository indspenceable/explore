using UnityEngine;
using System.Collections;

public class Readable : MonoBehaviour, IInteractable {
	[TextArea()]
	public string myText;
	public AudioClip blipSound;
	public void Interact() {
		StartCoroutine(GameManager.instance.Read(myText, blipSound));
	}
}
