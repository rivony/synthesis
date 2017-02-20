using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdableData {
	int textureSize = 512;
	const TextureFormat textureFormat = TextureFormat.RGB565;

	public Layer[] layers;

	float m_SavedMinHeight;
	float m_SavedMaxHeight;

	public void ApplyToMaterial(Material material){
		UpdateMeshHeight (material, m_SavedMinHeight, m_SavedMaxHeight);
	}

	public void UpdateMeshHeight(Material material, float minHeight, float maxHeight){

		//Debug.Log (baseColors.GetValue(1));
		material.SetColorArray (Constants.BASE_COLORS, layers.Select(l => l.tint).ToArray());
		material.SetInt (Constants.LAYER_COUNT, layers.Length);
		material.SetFloatArray (Constants.BASE_START_HEIGHTS, layers.Select(l => l.startHeight).ToArray());
		material.SetFloatArray (Constants.BASE_BLENDS, layers.Select(l => l.blendStrength).ToArray());
		material.SetFloatArray (Constants.BASE_COLOR_STRENGTH, layers.Select(l => l.tintStrength).ToArray());
		material.SetFloatArray (Constants.BASE_TEXTURE_SCALES, layers.Select(l => l.textureScale).ToArray());
		Texture2DArray textureArray = GenerateTextureArray (layers.Select (l => l.texture).ToArray ());
		material.SetTexture (Constants.BASE_TEXTURES, textureArray);


		m_SavedMinHeight = minHeight;
		m_SavedMaxHeight = maxHeight;

		material.SetFloat (Constants.MIN_HEIGHT, minHeight);
		material.SetFloat (Constants.MAX_HEIGHT, maxHeight);
	}

	Texture2DArray GenerateTextureArray(Texture2D[] textures){
		Texture2DArray textureArray = new Texture2DArray (textureSize, textureSize, textures.Length, 
			textureFormat, true);
		for (int i = 0; i < textures.Length; i++) {
			textureArray.SetPixels (textures [i].GetPixels (), i);
		}
		textureArray.Apply ();
		return textureArray;
	}
}
