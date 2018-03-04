using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public Texture2D front, side, top;
	public GameObject wall, floor;
	public Vector3 maxSize;

	public int blockCount;

	GameObject building;

	// xy = front, zy = side, xz = top
	bool[,] xy, zy, xz, xyOOB, zyOOB, xzOOB;

	void Start () {
		xy = MapArray(front);
		zy = MapArray(side);
		xz = MapArray(top);

		xyOOB = MapOOB(front);
		zyOOB = MapOOB(side);
		xzOOB = MapOOB(top);

		StartCoroutine(Build());
	}

	IEnumerator Build () {
		building = new GameObject("Building");
		for (int x = 0; x < maxSize.x; x++) {
			yield return new WaitForEndOfFrame();
			for (int z = 0; z < maxSize.z; z++) {
				for (int y = 0; y < maxSize.y; y++) {
					if (xz[x, z] && !xyOOB[x, y] && !zyOOB[z, y]) {
						var g = Instantiate(wall, new Vector3(x, y, z), Quaternion.identity);
						g.transform.parent = building.transform;
						blockCount++;
					} else if ((xy[x, y] || zy[z, y]) && !xzOOB[x, z] && !xyOOB[x, y] && !zyOOB[z, y]) {
						var g = Instantiate(floor, new Vector3(x, y, z), Quaternion.identity);
						g.transform.parent = building.transform;
						blockCount++;
					}
				}
			}
		}
	}

	bool[,] MapArray (Texture2D t) {
		bool[,] a = new bool[t.width, t.height];
		for (int x = 0; x < t.width; x++) {
			for (int y = 0; y < t.height; y++) {
				a[x, y] = t.GetPixel(x, y) == Color.black;
			}
		}
		return a;
	}

	bool[,] MapOOB (Texture2D t) {
		bool[,] a = new bool[t.width, t.height];
		for (int x = 0; x < t.width; x++) {
			for (int y = 0; y < t.height; y++) {
				var c = t.GetPixel(x, y);
				a[x, y] = c.r > 0.5f && c != Color.white;
			}
		}
		return a;
	}
}
