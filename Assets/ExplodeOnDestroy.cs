using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDestroy : MonoBehaviour {
	public GameObject ExplosionPrefab;
	public void OnDestroy() {
		Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, GameManager.instance.currentActiveObjects.transform);
	}
}
