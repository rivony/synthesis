[System.Serializable]
public struct LODInfo {
	public int LevelOfDetail;
	public float visibleDistanceThreshold; //distance within the LOD is active
	public bool useForCollider;
}
