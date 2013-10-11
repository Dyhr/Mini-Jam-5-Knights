using UnityEngine;
using System.Collections;

namespace grid {
	public class Grid : MonoBehaviour {
		private Color gizmoSolidColor = Color.white * 0.5f;
		private Color gizmoWireColor = Color.white * 0.7f;
		
		public int width = 1;
		public int height = 1;
		public float lowerDisplacement = 2;
		public Transform tile = null;
		
		private Vector3 tileSize;
		private float[] grid;
		
		private void Start () {
			if(width <= 0 || height <= 0){
				throw(new UnityException("Grid: Invalid size."));
			}
			if(tile == null){
				throw(new UnityException("Grid: No tile selected."));
			}
			
			LoadTileSize();
			
			grid = new float[width * height];
			int i = grid.Length;
			Transform t;
			while(i-- > 0){
				t = Instantiate(tile) as Transform;
				t.position = transform.position + new Vector3(
					(i%width - ((float)width/2) + 0.5f) * tileSize.x,0,
					((int)(i/width) - ((float)height/2) + 0.5f) * tileSize.y);
				t.parent = transform;
			}
		}
		
		private void Update () {
		
		}
		
		private void LoadTileSize(){
			if(tile != null) {
				tileSize = new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
						tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
						tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
			} else {
				tileSize = Vector3.one;
			}
		}
		
		// Gizmo functions
		private void OnDrawGizmos() {
			if(tileSize.magnitude == 0) {
				LoadTileSize();
			}
			Gizmos.color = gizmoSolidColor;
			Gizmos.DrawCube(transform.position,new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
		}
		private void OnDrawGizmosSelected() {
			if(tileSize.magnitude == 0) {
				LoadTileSize();
			}
			Gizmos.color = gizmoWireColor;
			Gizmos.DrawWireCube(transform.position,new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
		}
	}
	
	public abstract class Tile : MonoBehaviour {
		
	}
}
