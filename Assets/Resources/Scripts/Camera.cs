using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
    public static ArrayList players;
    public static Vector3 up = Vector3.forward;
    public static Vector3 right = Vector3.right;

	// Use this for initialization
	void Start () {
        if(players == null)
            players = new ArrayList(4);
	}
	
	// Update is called once per frame
	void Update () {
        if (players.Count > 0) {
            Vector3 average = Vector3.zero;
            foreach (Player p in players) {
                average += p.transform.position;
            }
            average /= players.Count;
            transform.position = Vector3.Lerp(transform.position,average + Vector3.one * 10,0.2f);
            transform.LookAt(transform.position - Vector3.one);

            right = transform.right;
            up = transform.forward;
        }
	}
}
