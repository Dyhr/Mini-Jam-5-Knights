using UnityEngine;
using System.Collections;

namespace Character {
	public enum Index {
	    One = 1,
	    Two = 2,
	    Three = 3,
	    Four = 4
	}
	
	[AddComponentMenu("Game/Player")]
	public class Player : MonoBehaviour {
	    public Index index;
		
		private Vector3 velocity;		
		private bool _alive;
		private bool _grounded;
		
		public bool alive { get{ return _alive; } }
		public bool grounded { get{ return _grounded; } }
		
		void Start () {
			switch ((int)index) {
	            case 1:
	                renderer.material.color = Color.Lerp(Color.white, Color.red, 0.6f);
	                break;
	            case 2:
	                renderer.material.color = Color.Lerp(Color.white, Color.blue, 0.6f);
	                break;
	            case 3:;
	                renderer.material.color = Color.Lerp(Color.white, Color.green, 0.6f);
	                break;
	            case 4:
	                renderer.material.color = Color.Lerp(Color.white, Color.yellow, 0.6f);
	                break;
	        }
			_alive = true;
		}
		
		void Update () {
			if(!_alive)return;
			velocity = new Vector3(0,-1,0)*Time.deltaTime;
			transform.Translate(velocity);
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Grid")){
				_grounded = g.GetComponent<Grid.Grid>().CollidePlayer(this);
				if(_grounded){
					break;
				}
			}
		}
	    private void OnDestroy() {
	        _alive = false;
		}
	}
	
	public class Control {
		
	}
}
