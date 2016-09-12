using UnityEngine;
using System.Collections;

public class VoiceCommandOpenDoor : MonoBehaviour, IVoiceReciever {
	public string command;
	public GameObject door;
	public void RecieveString(string s) {
		if (s == command) {
			Debug.Log("Yes!");
			Destroy(door);
		}
	}
}
