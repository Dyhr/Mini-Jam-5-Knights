using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Grid {
	
	[AddComponentMenu("Game/Grid")]
	public sealed class Grid : MonoBehaviour {
		
		// Constants:
		private float gizmoSolidAlpha = 0.7f;
		private Color gizmoSolidColor = Color.white * 0.7f;
		private float gizmoWireAlpha = 0.7f;
		private Color gizmoWireColor = Color.white * 0.8f;
		private Color gizmoDisplacedColor = Color.white * 0.5f;
		
		// Properties:
		public bool hideLowTiles = true;
		
		// Variables:
		private Generator gen;
		private int width = 1;
		private int height = 1;
		private float displacement = 2;
		private Vector3 tileSize;
		private Transform[] grid;
		
		// Init:
		private void Start () {
			// Setup the grid:
			width = Mathf.Max(1,Mathf.FloorToInt(transform.localScale.x));
			height = Mathf.Max(1,Mathf.FloorToInt(transform.localScale.z));
			displacement = transform.localScale.y;
			grid = new Transform[width * height];
			
			// Setup the tiles:
			gen = GetGenerator();
			tileSize = gen.GetTileSize();
			int i = grid.Length;
			while(i-- > 0){
				AddTile(i);
			}
		}
		
		// Public functions:
		public void AddController(Controller controller){
			if(grid == null){
				Start();
			}
			
			int i = grid.Length;
			while(i-- > 0){
				if(grid[i] == null){
					AddTile(i);
				}
				grid[i].GetComponent<Tile>().AddController(controller);
			}
		}
		
		// Private functions:
		private Generator GetGenerator(){
			Generator[] gens = GetComponents<Generator>();
			if(gens.Length == 0){
				throw(new UnityException("Grid: No generator found."));
			} else if(gens.Length > 1){
				throw(new UnityException("Grid: Too many generators found."));
			}
			gens[0].Init();
			return gens[0];
		}
		private Vector2 Convert(int i) {
			return new Vector2((i%width - ((float)width/2) + 0.5f) * tileSize.x,
					((int)(i/width) - ((float)height/2) + 0.5f) * tileSize.y);
		}
		private void AddTile(int i) {
			Transform t = Instantiate(gen.GetTile(new Vector2(tileSize.x,tileSize.z),Convert(i))) as Transform;
			t.position = transform.position + new Vector3(
					(i%width - ((float)width/2) + 0.5f) * tileSize.x,0,
					((int)(i/width) - ((float)height/2) + 0.5f) * tileSize.y);
			t.parent = transform;
			t.GetComponent<Tile>().Init(t.position - Vector3.up*displacement, t.position);
			t.position -= Vector3.up*displacement;
			
			grid[i] = t;
		}
		
		// Gizmo functions:
		private void OnDrawGizmos() {
			if(gen == null){
				gen = GetGenerator();
			}
			if(tileSize.magnitude == 0) {
				tileSize = gen.GetTileSize();
			}
			width = Mathf.Max(0,Mathf.FloorToInt(transform.localScale.x));
			height = Mathf.Max(0,Mathf.FloorToInt(transform.localScale.z));
			displacement = transform.localScale.y;
			
			gizmoSolidColor.a = gizmoSolidAlpha;
			Gizmos.color = gizmoSolidColor;
			Gizmos.DrawCube(transform.position,
					new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
		}
		private void OnDrawGizmosSelected() {
			if(gen == null){
				gen = GetGenerator();
			}
			if(tileSize.magnitude == 0) {
				tileSize = gen.GetTileSize();
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
	
	public abstract class Generator : MonoBehaviour {
		public abstract void Init();
		public abstract Transform GetTile(Vector2 size, Vector2 pos);
		public abstract Vector3 GetTileSize();
	}
	
	public abstract class Tile : MonoBehaviour {
		
		// Properties:
		private Vector3 upperPosition;
		private Vector3 lowerPosition;
		
		// Variables:
		private List<Controller> controllers;
		private float jitter;
		
		// Internals:
		internal void Init(Vector3 lowerPosition, Vector3 upperPosition){
			this.lowerPosition = lowerPosition;
			this.upperPosition = upperPosition;
			
			this.controllers = new List<Controller>();
			this.jitter = 0.1f;
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
			if(controllers == null){
				controllers = new List<Controller>();
			}
			
			// Sum up the influence of nearby controllers:
			float sum = 0;
			float inf;
			int i = controllers.Count;
			while(i-- > 0){
				inf = controllers[i].GetInfluence(upperPosition);
				if(inf > 0) {
					sum += inf;
				} else {
					controllers.RemoveAt(i); // Remove controller if tile is outside it's influence
				}
			}
			
			if(sum == 0){
				transform.position = Vector3.Lerp(transform.position,lowerPosition,0.3f);
			} else {
				// Take the average move tile to corresponding height:
				sum /= controllers.Count;
				transform.position = Vector3.Lerp(transform.position,Vector3.Lerp(Vector3.Lerp(lowerPosition,upperPosition,sum),transform.position+Vector3.up*(Random.value-0.5f),jitter),0.3f);
			}
			
			// Destroy the tile if it's outside any influence and it's at 0 to preserve some cpu:
			if(Vector3.Distance(transform.position,lowerPosition) < 0.0001f && controllers.Count == 0){
				Destroy(gameObject);
			}
		}
	}
	
	public abstract class Controller : MonoBehaviour {
		
		// Variables:
		private Vector3 prevPosition = Vector3.zero;
		
		// Internals:
		internal abstract float GetInfluence(Vector3 pos);
		
		// Private functions:
		private void Update(){
			if(transform.position != prevPosition){
				GameObject.Find("Grid").GetComponent<Grid>().AddController(this);
			}
			prevPosition = transform.position;
		}
	}
}
