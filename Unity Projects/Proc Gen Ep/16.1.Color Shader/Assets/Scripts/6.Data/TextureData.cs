using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdableData {

	public Color[] baseColours;
	[Range(0,1)]
	public float[] baseStartHeights;

	float savedMinHeight;
	float savedMaxHeight;

	public void ApplyToMaterial(Material material){
		UpdateMeshHeight (material, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeight(Material material, float minHeight, float maxHeight){
		//before update
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;
		//update
		material.SetFloat ("minHeight", minHeight);
		material.SetFloat ("maxHeight", maxHeight);
		material.SetInt ("baseColourCount", baseColours.Length);
		material.SetColorArray ("baseColours", baseColours);
		material.SetFloatArray("baseStartHeights", baseStartHeights);
	}
}
