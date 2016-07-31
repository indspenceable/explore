using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class LevelBuilder : MonoBehaviour {
	public Vector2 mapSize = new Vector2(80, 80);
	public Vector2 gridSize = new Vector2(1f, 1f);
	public Texture2D tileSheet;
	public Sprite[] sprites;
	public GameObject tilePrefab;

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
	[SerializeField]
	public List<TileLocation> tiles = new List<TileLocation>();
	public GameObject TileContainer() {
		if (_tc == null) {
			try {
				_tc = transform.FindChild("TileContainer").gameObject;
			} catch {
			}
		}
		if (_tc == null) {
			_tc = new GameObject();
			_tc.transform.parent = transform;
		}
		return _tc;
	}
	public GameObject FindOrCreateTileAt(int x, int y) {
		GameObject go = FindTileAt(x, y);
		if (go == null) {
			go = Instantiate(tilePrefab) as GameObject;
			go.transform.parent = TileContainer().transform;
			go.transform.localPosition = new Vector3(x, y);
			tiles.Add(new TileLocation(x, y, go));
		}
		return go;
	}

	public GameObject FindTileAt(int x, int y) {
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y) {
				return tl.tile;
			}
		}
		return null;
	}

	public void RemoveTileAt(int x, int y) {
		foreach (TileLocation tl in tiles) {
			if (tl.x == x && tl.y == y) {
				GameObject.DestroyImmediate(tl.tile);
				tiles.Remove(tl);
				return;
			}
		}
	}

#if UNITY_EDITOR
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
	}
	void SetSprites() {
	}
#endif
}
