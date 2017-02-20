using System;
using System.Threading;

public struct MapThreadInfo<T> {//T to handle the mapData and the meshData as well
	public readonly Action<T> Callback; //struct should be immutable
	public readonly T Parameter; //once we create them, we cannot change the values

	public MapThreadInfo (Action<T> callback, T parameter)
	{
		Callback = callback;
		Parameter = parameter;
	}
}
