using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace grid {
	public class Grid : MonoBehaviour {
		
		// Constants:
		private float gizmoSolidAlpha = 0.5f;
		private Color gizmoSolidColor = Color.white * 0.7f;
		private float gizmoWireAlpha = 0.5f;
		private Color gizmoWireColor = Color.white * 0.8f;
		private Color gizmoDisplacedColor = Color.white * 0.5f;
		
		// Properties:
		public int width = 1;
		public int height = 1;
		public float displacement = 2;
		public Transform tile = null;
		
		// Variables:
		private Vector3 tileSize;
		private Transform[] grid;
		
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
			grid = new Transform[width * height];
			int i = grid.Length;
			while(i-- > 0){
				AddTile(i);
			}
		}
		
		// Public functions:
		public void AddController(Controller controller){
			int i = grid.Length;
			while(i-- > 0){
				if(grid[i] == null){
					AddTile(i);
				}
				grid[i].GetComponent<Tile>().AddController(controller);
			}
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
		private void AddTile(int i) {
			Transform t = Instantiate(tile) as Transform;
			t.position = transform.position + new Vector3(
					(i%width - ((float)width/2) + 0.5f) * tileSize.x,0,
					((int)(i/width) - ((float)height/2) + 0.5f) * tileSize.y);
			t.parent = transform;
			t.GetComponent<Tile>().Init(i,t.position - Vector3.up*displacement, t.position);
			t.position -= Vector3.up*displacement;
			
			grid[i] = t;
		}
		private void HideTile(int index){
			Destroy(grid[index].gameObject);
			grid[index] = null;
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
		
		// Properties:
		private int index;
		private Vector3 upperPosition;
		private Vector3 lowerPosition;
		
		// Variables:
		private List<Controller> controllers;
		
		// Internals:
		internal void Init(int index, Vector3 lowerPosition, Vector3 upperPosition){
			this.index = index;
			this.lowerPosition = lowerPosition;
			this.upperPosition = upperPosition;
			
			this.controllers = new List<Controller>();
		}
		internal void AddController(Controller controller){
			if(controllers == null){
				controllers = new List<Controller>();
			}
			if(controller.GetInfluence(upperPosition) > 0){
				controllers.Add(controller);
			}
		}
		
		// Private functions:
		private void Update(){
			float sum = 0;
			int i = controllers.Count;
			while(i-- > 0){
				sum += controllers[i].GetInfluence(upperPosition);
			}
			if(sum == 0){
				transform.position = Vector3.Lerp(transform.position,lowerPosition,0.3f);
			} else {
				sum /= controllers.Count;
				transform.position = Vector3.Lerp(transform.position,Vector3.Lerp(lowerPosition,upperPosition,sum),0.3f);
			}
			
			if(Vector3.Distance(transform.position,lowerPosition) < 0.0001f && controllers.Count == 0){
				transform.parent.SendMessage("HideTile", index);
			}
		}
	}
	
	public abstract class Controller : MonoBehaviour {
		
		// Properties:
		protected float radius = 3;
		
		// Internals:
		internal abstract float GetInfluence(Vector3 pos);
		
		
		// Gizmo functions:
		private void OnDrawGizmos() {
			Gizmos.color = Color.black;
			float inc = (Mathf.PI*2)/180;
			float angle = Mathf.PI*2;
			while(angle >= 0){
				Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
						transform.position + new Vector3(Mathf.Cos(angle-inc),0,Mathf.Sin(angle-inc))*radius);
				angle -= inc;
			}
			angle = Mathf.PI*2;
			while(angle >= 0){
				Gizmos.DrawLine(transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
						transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle-inc),0,Mathf.Sin(angle-inc))*radius);
				angle -= inc;
			}
			inc = (Mathf.PI*2)/8;
			angle = Mathf.PI*2;
			while(angle >= 0){
				Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius,
						transform.position + Vector3.up*3 + new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*radius);
				angle -= inc;
			}
		}
	}
}
