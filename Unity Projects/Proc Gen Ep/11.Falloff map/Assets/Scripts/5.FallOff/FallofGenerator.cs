using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallofGenerator {
	
	public static float[,] GenerateFalloffMap(int size){
		float[,] map = new float[size, size];
		float x;
		float y;
		float sizef = (float)size;
		float value;

		for (int iCoord = 0; iCoord < size; iCoord++) {
			for (int jCoord = 0; jCoord < size; jCoord++) {
				x = iCoord / sizef;
				y = jCoord / sizef;
				//convert the value range(0,1) to range(-1,1)
				x = x * 2 - 1;
				y = y * 2 - 1;

				//which is closest to the edge of the square?
				value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

				map [iCoord, jCoord] = Evaluate(value);
			}
		}

		return map;
	}

	static float Evaluate(float value){
		float a = 3;
		float b = 2.2f;
		//fonction to increase the darker zone in the falloff map
		//f(x) = x^a / ( x^a + (b - bx)^a ) from f(x) = x^a / ( x^a + (1-x)^a )
		float pow_a = Mathf.Pow(value, a);
		return pow_a / (pow_a + Mathf.Pow(b - b * value, a));
	}
}
