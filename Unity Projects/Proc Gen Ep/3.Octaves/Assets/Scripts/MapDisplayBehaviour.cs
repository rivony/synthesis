using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayBehaviour : MonoBehaviour {

	public Renderer TextureRender;

	public void DrawNoiseMap(float[,] noiseMap){
		int width = noiseMap.GetLength (0);// first dimension length
		int height = noiseMap.GetLength(1);// second dimension length

		Texture2D texture = new Texture2D (width, height);

		Color[] colorMap = new Color[width * height];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colorMap [y * width + x] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
			}
		}

		texture.SetPixels (colorMap);
		texture.Apply ();// apply everything we've done

		TextureRender.sharedMaterial.mainTexture = texture; // Renderer.material vs Renderer.sharedMaterial (the first at runtime, the second in the editor)
		TextureRender.transform.localScale = new Vector3(width, 1, height); 
	}
}
