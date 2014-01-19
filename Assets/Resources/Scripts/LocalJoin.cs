using UnityEngine;
using System.Collections;

public class LocalJoin : MonoBehaviour {
	
	private const int maxPlayers = 4;
    private Hashtable players;
	private GameObject[] dummies;
	
	void Start () {
		players = new Hashtable(maxPlayers);
		dummies = GameObject.FindGameObjectsWithTag("Respawn");
	}
	
	void Update () {
		for (int i = 1; i <= maxPlayers; i++) {
            if (Input.GetKeyDown("joystick "+i+" button 0")) {
                if (!players.ContainsKey(i)) {
                    for (int j = 1; j <= maxPlayers; j++) {
                        if (!players.ContainsValue(j)) {
							dummies[j-1].renderer.material.color = getColor(j);
                            players.Add(i, j);
							Debug.Log("Player: " + i + " joined");
                            break;
                        }
                    }
                } else {
					dummies[((int)players[i])-1].renderer.material.color = Color.white;
                    players.Remove(i);
					Debug.Log("Player: " + i + " left");
                }
            }
            if (Input.GetKeyDown("joystick " + i + " button 7") && players.Keys.Count > 0) {
                int numPlayers = players.Keys.Count;
                if (numPlayers == 0)
                    return;
				
				// Make blackbox
				
				Debug.Log("Game Starting");
				Application.LoadLevel("Arena");
            }
        }
	}
	private Color getColor(int index){
		switch((int)index) {
	     	case 1:
	            return Color.Lerp(Color.white, Color.red, 0.6f);
	        case 2:
	            return Color.Lerp(Color.white, Color.blue, 0.6f);
	        case 3:;
	            return Color.Lerp(Color.white, Color.green, 0.6f);
	        case 4:
	            return Color.Lerp(Color.white, Color.yellow, 0.6f);
            default:
                return (Color.white) / 2;
	    }
	}
}
