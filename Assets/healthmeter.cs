using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthmeter : MonoBehaviour {
	public Image[] hearts;
	public void SetHealth(int amt) {
		for( int i = 0 ; i < hearts.Length; i += 1) {
			hearts[i].gameObject.SetActive(i < amt);
		}
	}
}
