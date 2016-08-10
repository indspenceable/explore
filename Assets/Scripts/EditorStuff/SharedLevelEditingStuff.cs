using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SharedLevelEditingStuff {
	#if UNITY_EDITOR
	public GameObject defaultTilePrefab;
	public List<string> knownTileSheets;
	public Material pixelPerfectSprite;

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
	#endif
}
