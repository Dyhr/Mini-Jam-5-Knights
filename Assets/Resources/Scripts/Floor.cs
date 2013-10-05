using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {
    private Stack removeIsland;
    private ArrayList islands;
    private Transform[,] transforms;
    private float[,] positions;
    private float[,] jitter;
    public uint width;
    public uint height;
    public Transform tile;

    void Start() {
        if (width == 0) width = 32;
        if (height == 0) height = 32;
        positions = new float[width, height];
        transforms = new Transform[width, height];
        jitter = new float[width, height];
        removeIsland = new Stack();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                GameObject g = (Instantiate(tile) as Transform).gameObject;
                g.transform.position = new Vector3(transform.position.x + i - width / 2, transform.position.y, transform.position.z + j - height / 2);
                g.transform.Rotate(0, 0, Mathf.Floor(Random.value * 4) * 90);
                transforms[i, j] = g.transform;
                positions[i, j] = transform.position.y - 5 + Mathf.PerlinNoise(g.transform.position.x / 2, g.transform.position.z / 2);
                jitter[i, j] = 0.1f;
            }
        }

        islands = new ArrayList();

        AddIsland(new Island(3, 3, 20, 20));
        AddIsland(new Island(13, 3, 20, 20));
        AddIsland(new Island(3, 13, 20, 20));
        AddIsland(new Island(13, 13, 20, 20));
    }

    void AddIsland(Island island) {
        islands.Add(island);
        for (int i = Mathf.Max(island.x,0); i < Mathf.Min(width, island.x + island.width); i++) {
            for (int j = Mathf.Max(island.y, 0); j < Mathf.Min(height, island.y + island.height); j++) {
                if (island.heights[i - island.x, j - island.y] != 0)
                    positions[i, j] = island.heights[i - island.x, j - island.y];
            }
        }
    }
    void RemoveIsland(Island island) {
        removeIsland.Push(island);
        for (int i = Mathf.Max(island.x, 0); i < Mathf.Min(width, island.x + island.width); i++) {
            for (int j = Mathf.Max(island.y, 0); j < Mathf.Min(height, island.y + island.height); j++) {
                if (island.heights[i - island.x, j - island.y] != 0)
                    positions[i, j] = transform.position.y - 5 + Mathf.PerlinNoise(transforms[i, j].position.x / 2, transforms[i, j].position.z / 2);
            }
        }
    }

    void Update() {
        Transform t;
        float dist;
        float move;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                t = transforms[i, j];
                dist = positions[i, j] - t.position.y;
                move = Mathf.Sign(dist) * Time.deltaTime * Mathf.Min(1, Mathf.Abs(dist));

                jitter[i,j] = 0.1f;
                foreach (Island island in islands) {
                    if (island.life < 5) {
                        if (i - island.x >= 0 && i - island.x < island.width) {
                            if (j - island.y >= 0 && j - island.y < island.height) {
                                if(island.heights[i - island.x, j - island.y] > 0)
                                    jitter[i,j] += (5-island.life)/5;
                            }
                        }
                    }
                }

                t.position = new Vector3(i, t.position.y + Mathf.Lerp(move, (Random.value - 0.5f) * Time.deltaTime * 4, jitter[i,j]), j);
                //t.position = new Vector3(i, Mathf.Min(Mathf.Max(positions[i, j] - 0.5f, t.position.y), positions[i, j] + 0.5f), j);
            }
        }

        foreach (Island i in islands) {
            i.life -= Time.deltaTime;
            if (i.life <= 0) {
                RemoveIsland(i);
            }
        }
        while (removeIsland.Count > 0) {
            islands.Remove(removeIsland.Pop() as Island);
            AddIsland(new Island(Mathf.FloorToInt(Random.value * 32) - 8, Mathf.FloorToInt(Random.value * 32) - 8, 20, 20));
            if(islands.Count < 18)
                AddIsland(new Island(Mathf.FloorToInt(Random.value * 32) - 8, Mathf.FloorToInt(Random.value * 32) - 8, 20, 20));
        }
    }
}
public class Island {
    public float life;
    public int x, y;
    public int width, height;
    public float[,] heights;

    public Island(int x, int y, int width, int height) {
        this.life = Random.value * 15 + 5;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        heights = new float[width, height];

        float sum = 0;
        float[,] random = new float[width, height];
        float[,] blur = new float[width, height];
        for (uint i = 1; i < width - 1; i++) {
            for (uint j = 1; j < height - 1; j++) {
                random[i, j] = Random.value;
            }
        }
        for (uint i = 1; i < width - 1; i++) {
            for (uint j = 1; j < height - 1; j++) {
                sum = 0;
                for (int k = -1; k <= 1; k++) {
                    for (int l = -1; l <= 1; l++) {
                        sum += random[i + k, j + l];
                    }
                }
                blur[i, j] = sum / 9;
            }
        }

        float h;
        for (uint i = 0; i < width; i++) {
            for (uint j = 0; j < height; j++) {
                h = ((width / 2) - Mathf.Sqrt((width / 2 - i) * (width / 2 - i) + (height / 2 - j) * (height / 2 - j))) / (width / 2);
                if (Mathf.Sqrt((width / 2 - i) * (width / 2 - i) + (height / 2 - j) * (height / 2 - j)) < width / 2) {
                    heights[i, j] = Mathf.Lerp(blur[i, j], h, h) * 2.5f;
                }
            }
        }

        int destroy = Mathf.FloorToInt(width/3f);
        int kills;
        for (int i = 0; i < width; i++) {
            kills = Mathf.FloorToInt((Random.value + Random.value) / 2 * destroy) + 1;
            for (int j = 0; j < height && kills > 0; j++) {
                if (heights[i, j] > 0) {
                    heights[i, j] = 0;
                    kills--;
                }
            }
            kills = Mathf.FloorToInt((Random.value + Random.value)/2 * destroy) + 1;
            for (int j = height - 1; j >= 0 && kills > 0; j--) {
                if (heights[i, j] > 0) {
                    heights[i, j] = 0;
                    kills--;
                }
            }
        }
        for (int i = 0; i < height; i++) {
            kills = Mathf.FloorToInt((Random.value + Random.value) / 2 * destroy) + 1;
            for (int j = 0; j < width && kills > 0; j++)
            {
                if (heights[j, i] > 0)
                {
                    heights[j, i] = 0;
                    kills--;
                }
            }
            kills = Mathf.FloorToInt((Random.value + Random.value) / 2 * destroy) + 1;
            for (int j = width - 1; j >= 0 && kills > 0; j--)
            {
                if (heights[j, i] > 0)
                {
                    heights[j, i] = 0;
                    kills--;
                }
            }
        }
    }
}
