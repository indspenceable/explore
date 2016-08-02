using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class LevelBuilder : MonoBehaviour {
#if UNITY_EDITOR
	public Vector2 mapSize = new Vector2(80, 80);
	public Vector2 gridSize = new Vector2(1f, 1f);
	public Texture2D tileSheet;
	public Sprite[] sprites;
	public GameObject defaultTilePrefab;
	public Sprite currentlySelectedSprite;

	private GameObject _tc;
	public struct TileLocation {
		public TileLocation(int _x, int _y, GameObject _tile) {
			x = _x;
			y = _y;
			tile = _tile;
		}
		public int x;
		public int y;
		public GameObject tile;
	}
	[System.Serializable]
	public class SpritePrefabPairing {
		public SpritePrefabPairing(Sprite sprite, GameObject prefab) {
			this.sprite = sprite;
			this.prefab = prefab;
		}
		public Sprite sprite;
		public GameObject prefab;
	}

	public SpritePrefabPairing LocateByCurrentSprite() {
		if (spritePrefabPairings == null) {
			spritePrefabPairings = new List<SpritePrefabPairing>();
		}
		foreach (SpritePrefabPairing spp in spritePrefabPairings) {
			if (spp.sprite == currentlySelectedSprite) {
				return spp;
			}
		}
		return null;
	}
	public GameObject CurrentPrefab() {
		SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			return spp.prefab;
		} else {
			return defaultTilePrefab;
		}
	}
	public void SetCurrentPrefab(GameObject prefab) {
		SpritePrefabPairing spp = LocateByCurrentSprite();
		if (spp != null) {
			spp.prefab = prefab;
		} else {
			spritePrefabPairings.Add(new SpritePrefabPairing(currentlySelectedSprite, prefab));
		}
	}

	[SerializeField]
	public List<SpritePrefabPairing> spritePrefabPairings;

	public List<TileLocation> tiles;

	public GameObject TileContainer() {
		if (_tc == null) {
			try {
				_tc = transform.FindChild("TileContainer").gameObject;
			} catch {
			}
		}
		if (_tc == null) {
			_tc = new GameObject("TileContainer");
			_tc.transform.parent = transform;
		}
		return _tc;
	}
	public GameObject FindOrCreateTileAt(int x, int y) {
		GameObject go = FindTileAt(x, y);
		if (go == null) {
			go = PrefabUtility.InstantiatePrefab(CurrentPrefab()) as GameObject;
			go.transform.parent = TileContainer().transform;
			go.transform.localPosition = new Vector3(x + gridSize.x/2, y + gridSize.y/2);
			tiles.Add(new TileLocation(x, y, go));
		}
		return go;
	}

	public GameObject FindTileAt(int x, int y) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y) {
				return tl.tile;
			}
		}
		return null;
	}
		
	public void RemoveTileAt(int x, int y) {
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y) {
				GameObject.DestroyImmediate(tl.tile);
				tiles.Remove(tl);
				return;
			}
		}
	}

	private void EnsureTileLocationListIsSetup() {
		if (tiles == null) {
			tiles = new List<TileLocation>();
			for (int i = 0; i < TileContainer().transform.childCount; i+=1) {
				Transform child = TileContainer().transform.GetChild(i);
				tiles.Add(new TileLocation(Mathf.RoundToInt(child.localPosition.x-gridSize.x/2), Mathf.RoundToInt(child.localPosition.y-gridSize.y/2), child.gameObject));
			}
		}
	}

	public void RebindChildrenToTheirPrefabs() {
		Sprite css = currentlySelectedSprite;
		EnsureTileLocationListIsSetup();
		foreach (TileLocation tl in tiles) {
			currentlySelectedSprite = tl.tile.GetComponent<SpriteRenderer>().sprite;
			if (PrefabUtility.GetPrefabParent(tl.tile) != CurrentPrefab()) {
				Debug.Log("Damn!");
			}
		}
		currentlySelectedSprite = css;
	}

	// in the editor, validate a bunch of things on change. But we rely on editor-specific
	// tools so don't compile it into the playable game.
	[ExecuteInEditMode]
	void OnValidate(){
		mapSize = new Vector2((int) mapSize.x, (int) mapSize.y);
		if (tileSheet) {
			string spriteSheet = AssetDatabase.GetAssetPath( tileSheet );
			sprites = AssetDatabase.LoadAllAssetsAtPath( spriteSheet )
				.OfType<Sprite>().ToArray();
			if (sprites.Length == 0) {
				Debug.LogError("Unable to set to non-multiSprite value.");
				tileSheet = null;
			}
		}
		RebindChildrenToTheirPrefabs();
	}
#endif
}
