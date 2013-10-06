using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {
    public static bool[] joined = new bool[4];

	void Start () {
	    
	}
	
	void Update () {
        for (int i = 0; i < 4; i++) {
            joined[i] = Input.GetButton("Player" + (i + 1) + "Attack");
        }
        if(Input.GetButton("Player1Jump")){
            GetComponent<Floor>().init();
            enabled = false;
        }
	}
}
