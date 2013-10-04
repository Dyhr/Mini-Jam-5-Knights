using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {
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
                transforms[i, j] = g.transform;
                positions[i, j] = transform.position.y - 5 + Random.value;
            }
        }
	}

    void Update()
    {
        Transform t;
        float dist;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                t = transforms[i, j];
                dist = positions[i, j] - t.position.y;
                
                t.position = new Vector3(i, t.position.y + Mathf.Sign(dist) * Time.deltaTime * Mathf.Min(1,Mathf.Abs(dist)), j);
            }
        }
	}
}
