using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSamplingTest : MonoBehaviour {
	[Range(0,100)]
	public int sampleCount = 1;

	public float radius = 1;
	public Vector2 regionSize = Vector2.one;
	[Range(1, 100)]

	public int rejectionSamples = 30;
	[Range(1, 100)]
		public float displayRadius =1;

	List<Vector2> points;

	void OnValidate() {
		GeneratePoints();
 	}


	public void GeneratePoints()
	{
		points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples, sampleCount);

	}

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(regionSize/2,regionSize);
		if (points != null) {
			foreach (Vector2 point in points) {
				Gizmos.DrawSphere(point, displayRadius);
			}
		}
	}
}
