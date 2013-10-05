using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {
    private Stack islands;
    private Transform[,] transforms;
    private float[,] positions;
    public uint width;
    public uint height;

    void Start()
    {
        GameObject tile = Resources.Load("tile") as GameObject;
        if (width == 0) width = 32;
        if (height == 0) height = 32;
        positions = new float[width, height];
        transforms = new Transform[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject g = Instantiate(tile) as GameObject;
                g.transform.position = new Vector3(transform.position.x + i - width/2, transform.position.y, transform.position.z + j - height/2);
                g.transform.Rotate(0, 0, Mathf.Floor(Random.value * 4) * 90);
                transforms[i, j] = g.transform;
                positions[i, j] = g.transform.position.y - 5 + Mathf.PerlinNoise(g.transform.position.x/2,g.transform.position.z/2);
            }
        }

        islands = new Stack();
        islands.Push(new Island(1,1,31,31));
        AddIsland(islands.Peek() as Island);
	}

    void AddIsland(Island island)
    {
        for (int i = island.x; i < Mathf.Min(width,island.x + island.width); i++)
        {
            for (int j = island.y; j < Mathf.Min(height, island.y + island.height); j++)
            {
                if(island.heights[i - island.x, j - island.y] != 0)
                    positions[i, j] = island.heights[i - island.x, j - island.y];
            }
        }
    }

    void Update()
    {
        Transform t;
        float dist;
        float move;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                t = transforms[i, j];
                dist = positions[i, j] - t.position.y;
                move = Mathf.Sign(dist) * Time.deltaTime * Mathf.Min(1,Mathf.Abs(dist));
                
                t.position = new Vector3(i, t.position.y + Mathf.Lerp(move,(Random.value-0.5f)*Time.deltaTime,0.3f), j);
            }
        }
	}
}
public class Island
{
    public int x, y;
    public int width, height;
    public float[,] heights;

    public Island(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        heights = new float[width, height];

        float sum = 0;
        float[,] random = new float[width, height];
        float[,] blur = new float[width, height];
        for (uint i = 1; i < width - 1; i++)
        {
            for (uint j = 1; j < height - 1; j++)
            {
                random[i, j] = Random.value;
            }
        }
        for (uint i = 1; i < width - 1; i++)
        {
            for (uint j = 1; j < height - 1; j++)
            {
                sum = 0;
                for (int k = -1; k <= 1; k++)
                {
                    for (int l = -1; l <= 1; l++)
                    {
                        sum += random[i + k, j + l];
                    }
                }
                blur[i, j] = sum / 9;
            }
        }

        for (uint i = 0; i < width; i++)
        {
            for (uint j = 0; j < height; j++)
            {
                if (Mathf.Sqrt((width / 2 - i) * (width / 2 - i) + (height / 2 - j) * (height / 2 - j)) < width / 2)
                {
                    heights[i, j] = 1;
                }
            }
        }

        float h;
        for (uint i = 1; i < width - 1; i++)
        {
            for (uint j = 1; j < height - 1; j++)
            {
                h = ((width / 2) - Mathf.Sqrt((width / 2 - i) * (width / 2 - i) + (height / 2 - j) * (height / 2 - j))) / (width / 2);
                if(heights[i,j] != 0)
                    heights[i,j] = Mathf.Lerp(blur[i, j],h,h)*3;
            }
        }
    }
}
