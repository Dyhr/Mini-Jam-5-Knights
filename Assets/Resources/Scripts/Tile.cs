using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    private int i, j;
    private bool inited = false;

	void Start () {
	
	}

    public void init(int x, int y) {
        this.i = x;
        this.j = y;
        this.inited = true;
    }
	
	void Update () {
        if (!inited)
            return;
        if (transform.position.y < 0f && Floor.positions[i, j] < 0f)
            return;

        float dist = Floor.positions[i, j] - transform.position.y;
        float move = Mathf.Sign(dist) * Time.deltaTime * Mathf.Min(1, Mathf.Abs(dist));

        Floor.jitter[i, j] = 0.1f;
        foreach (Island island in Floor.islands) {
            if (island.life < 5) {
                if (i - island.x >= 0 && i - island.x < island.width) {
                    if (j - island.y >= 0 && j - island.y < island.height) {
                        if (island.heights[i - island.x, j - island.y] > 0)
                            Floor.jitter[i, j] += (5 - island.life) / 5;
                    }
                }
            }
        }

        transform.position = new Vector3(i, transform.position.y + Mathf.Lerp(move, (Random.value - 0.5f) * Time.deltaTime * 4, Floor.jitter[i, j]), j);
	}
}
