using UnityEngine;
using System.Collections;

public class Readable : MonoBehaviour {
	[TextArea()]
	public string myText;
	public void Interact() {
		Debug.Log("Hi?");
		StartCoroutine(GameManager.instance.Read(myText));
	}
}
