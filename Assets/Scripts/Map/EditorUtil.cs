using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
#endif

public class EditorUtil : MonoBehaviour
{
#if UNITY_EDITOR
	public Sprite[] _sprites;
	public Sprite[] sprites{
		get {
			if (_sprites == null || _sprites.Length == 0) {
				if (knownTileSheets.Count > 0) {
					SetCurrentTileSheet(knownTileSheets[0]);
				} else {
					_sprites = new Sprite[]{};
				}

			}
			return _sprites;
		}
		set {
			_sprites = value;
		}
	}
	public Sprite currentlySelectedSprite;
	public GameObject currentlySelectedPrefab;

	public int currentLayer;
	public string currentlySelectedTileSheetAssetLocation;
	public List<string> knownTileSheets;
	public Material pixelPerfectSprite;

	public bool CurrentLayerNeedsCollider() {
		return currentLayer == 2;
	}
	public bool CurrentLayerIsPrefabs() {
		return currentLayer == 5;
	}

	public void SetCurrentTileSheet(string target) {
		currentlySelectedTileSheetAssetLocation = target;
		knownTileSheets.Add(currentlySelectedTileSheetAssetLocation);
		knownTileSheets = knownTileSheets.Distinct().ToList();

		Debug.Log(currentlySelectedTileSheetAssetLocation);
		sprites = AssetDatabase.LoadAllAssetsAtPath( currentlySelectedTileSheetAssetLocation )
			.OfType<Sprite>().ToArray();
	}

	public static void DrawTexture(Rect position, Sprite sprite, Vector2 size)
	{
		Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
			sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
		Vector2 actualSize = size;

		actualSize.y *= (sprite.rect.height / sprite.rect.width);
		Graphics.DrawTexture(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect, 0, 0, 0, 0);
	}

	public static void DrawTextureGUI(Rect position, Sprite sprite, Vector2 size)
	{
		if (sprite == null) {
			return;
		}
		Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, 
			sprite.rect.y / sprite.texture.height,
			sprite.rect.width / sprite.texture.width, 
			sprite.rect.height / sprite.texture.height);
		Vector2 actualSize = size;

		actualSize.y *= (sprite.rect.height / sprite.rect.width);
		GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
	}
#endif
}

