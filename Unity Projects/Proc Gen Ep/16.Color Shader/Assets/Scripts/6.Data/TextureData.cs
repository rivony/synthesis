using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdableData {

	float m_SavedMinHeight;
	float m_SavedMaxHeight;

	public Color[] baseColors;
	//Starting heights for each color
	[Range(0, 1)]
	public float[] baseStartHeights;

	public void ApplyToMaterial(Material material){
		UpdateMeshHeight (material, m_SavedMinHeight, m_SavedMaxHeight);
	}

	public void UpdateMeshHeight(Material material, float minHeight, float maxHeight){

		//Debug.Log (baseColors.GetValue(1));
		material.SetColorArray (Constants.BASE_COLORS, baseColors);
		material.SetInt (Constants.BASE_COLORS_COUNT, baseColors.Length);
		material.SetFloatArray (Constants.BASE_START_HEIGHTS, baseStartHeights);

		m_SavedMinHeight = minHeight;
		m_SavedMaxHeight = maxHeight;

		material.SetFloat (Constants.MIN_HEIGHT, minHeight);
		material.SetFloat (Constants.MAX_HEIGHT, maxHeight);
	}
}
