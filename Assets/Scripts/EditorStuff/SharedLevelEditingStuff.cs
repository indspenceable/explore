using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class SharedLevelEditingStuff {
	public GameObject defaultTilePrefab;
	public List<string> knownTileSheets;

	[System.Serializable]
	public class SpritePrefabPairing {
		public SpritePrefabPairing(Sprite sprite, GameObject prefab) {
			this.sprite = sprite;
			this.prefab = prefab;
		}
		public Sprite sprite;
		public GameObject prefab;
	}

	[SerializeField]
	public List<SpritePrefabPairing> spritePrefabPairings;
}
