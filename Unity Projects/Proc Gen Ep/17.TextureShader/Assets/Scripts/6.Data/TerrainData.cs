using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdableData {
	const float TIME_ZERO = 0f;
	const float TIME_ONE = 1f;

	public float UniformScale = 2.5f;

	public bool UseFlatShading;
	public bool UseFalloff;

	public float MeshHeightMultiplier; //to elevate the heights
	public AnimationCurve MeshHeightCurve; //to flatten the water

	public float MinHeight {
		get{ 
			return UniformScale * MeshHeightMultiplier * MeshHeightCurve.Evaluate (TIME_ZERO);
		}
	}

	public float MaxHeight {
		get{ 
			return UniformScale * MeshHeightMultiplier * MeshHeightCurve.Evaluate (TIME_ONE);
		}
	}
}
