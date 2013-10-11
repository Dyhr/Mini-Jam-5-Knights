using UnityEngine;
using System.Collections;

namespace grid {
	public class Grid : MonoBehaviour {
		
		// Constants:
		private float gizmoSolidAlpha = 0.5f;
		private Color gizmoSolidColor = Color.white * 0.7f;
		private float gizmoWireAlpha = 0.5f;
		private Color gizmoWireColor = Color.white * 0.8f;
		private Color gizmoDisplacedColor = Color.white * 0.5f;
		
		// Settings:
		public int width = 1;
		public int height = 1;
		public float displacement = 2;
		public Transform tile = null;
		
		// Variables:
		private Vector3 tileSize;
		private float[] grid;
		
		// Init:
		private void Start () {
			// Check for invalid parameters:
			if(width <= 0 || height <= 0){
				throw(new UnityException("Grid: Invalid size."));
			}
			if(tile == null){
				throw(new UnityException("Grid: No tile selected."));
			}
			if(tile.GetComponent<Tile>() == null){
				throw(new UnityException("Grid: Tile doesn't contain a tile script."));
			}
			
			// Setup the tiles:
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
				t.GetComponent<Tile>().upperPosition = t.position;
				t.GetComponent<Tile>().lowerPosition = t.position - Vector3.up*displacement;
			}
		}
		
		// Public functions:
		public void AddController(){
			
		}
		
		// Private functions:
		private void LoadTileSize(){
			if(tile != null) {
				tileSize = new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
						tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
						tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
			} else {
				tileSize = Vector3.one;
			}
		}
		
		// Gizmo functions:
		private void OnDrawGizmos() {
			if(tileSize.magnitude == 0) {
				LoadTileSize();
			}
			gizmoSolidColor.a = gizmoSolidAlpha;
			Gizmos.color = gizmoSolidColor;
			Gizmos.DrawCube(transform.position,
					new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
		}
		private void OnDrawGizmosSelected() {
			if(tileSize.magnitude == 0) {
				LoadTileSize();
			}
			gizmoWireColor.a = gizmoWireAlpha;
			gizmoDisplacedColor.a = gizmoWireAlpha;
			Gizmos.color = gizmoWireColor;
			Gizmos.DrawWireCube(transform.position,
					new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
			Gizmos.color = gizmoDisplacedColor;
			Gizmos.DrawWireCube(transform.position - Vector3.up*displacement,
					new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
		}
	}
	
	public abstract class Tile : MonoBehaviour {
		internal Vector3 upperPosition;
		internal Vector3 lowerPosition;
		
		private void Update(){
			transform.position = Vector3.Lerp(transform.position,lowerPosition,0.3f);
		}
	}
	
	public abstract class Controller {
	}
}
