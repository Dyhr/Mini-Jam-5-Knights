using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {
    private bool inited = false;
    private Stack removeIsland;
    public static ArrayList islands;
    public static Transform[,] transforms;
    public static float[,] positions;
    public static float[,] jitter;
    public static uint width;
    public static uint height;
    public Transform tile;
    public float platformInit = 2;

    void Start() {
        
    }
    public void init() {
        if (width == 0) width = 32;
        if (height == 0) height = 32;
        positions = new float[width, height];
        transforms = new Transform[width, height];
        jitter = new float[width, height];
        removeIsland = new Stack();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                GameObject g = (Instantiate(tile) as Transform).gameObject;
                g.transform.position = new Vector3(transform.position.x + i - width / 2, transform.position.y + platformInit, transform.position.z + j - height / 2);
                g.transform.Rotate(0, 0, Mathf.Floor(Random.value * 8) * 45);
                g.renderer.material.color = Color.Lerp(Color.Lerp(Color.white, Color.white * 0.2f, Mathf.PerlinNoise(g.transform.position.x / 4, g.transform.position.z / 4)),
                    Color.Lerp(Color.white, Color.white * 0.2f, Random.value), 0.4f);
                g.AddComponent<Tile>();
                g.GetComponent<Tile>().init(i, j);
                transforms[i, j] = g.transform;
                positions[i, j] = transform.position.y - 5 + Mathf.PerlinNoise(g.transform.position.x / 2, g.transform.position.z / 2);
                jitter[i, j] = 0.1f;
            }
        }

        islands = new ArrayList();

        AddIsland(new Island(-1, -1, 20, 20));
        AddIsland(new Island(13, -1, 20, 20));
        AddIsland(new Island(-1, 13, 20, 20));
        AddIsland(new Island(13, 13, 20, 20));

        Player[] players = (Player[])GameObject.FindObjectsOfType(typeof(Player));
        foreach (Player p in players) {
            p.makeHidden(p.transform, "Players");
            p.ali = true;
            if (!StartUp.joined[(int)((p.GetComponent<Player>()).index) - 1]) {
                Destroy(p.gameObject);
            }
        }

        inited = true;
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
        if (!inited)
            return;
        foreach (Island i in islands) {
            i.life -= Time.deltaTime;
            if (i.life <= 0) {
                RemoveIsland(i);
            }
        }
        while (removeIsland.Count > 0) {
            islands.Remove(removeIsland.Pop() as Island);
            AddIsland(new Island(Mathf.FloorToInt(Random.value * 20) - 6, Mathf.FloorToInt(Random.value * 20) - 6, 16 + Mathf.FloorToInt(Random.value * 6), 16 + Mathf.FloorToInt(Random.value * 6)));
            if(islands.Count < 12)
                AddIsland(new Island(Mathf.FloorToInt(Random.value * 20) - 6, Mathf.FloorToInt(Random.value * 20) - 6, 16 + Mathf.FloorToInt(Random.value * 6), 16 + Mathf.FloorToInt(Random.value * 6)));
        }
    }
}
public class Island {
    public float life;
    public int x, y;
    public int width, height;
    public float[,] heights;

    public Island(int x, int y, int width, int height) {
        this.life = Random.value * 12 + 6;
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
