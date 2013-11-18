using UnityEngine;using System.Collections;
using System.Collections.Generic;

namespace Grid {
	
	[AddComponentMenu("Game/Grid")]
	public sealed class Grid : MonoBehaviour {
		
		// Constants:
		private readonly Color gizmoSolidColor = Color.white * 0.7f * new Color(1,1,1,0.8f);
		private readonly Color gizmoWireColor = Color.white * 0.8f * new Color(1,1,1,0.8f);
		private readonly Color gizmoDisplacedColor = Color.white * 0.5f* new Color(1,1,1,0.8f);
		
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
			gameObject.tag = "Grid";
			
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
		public bool CollidePlayer(Character.Player player){
			Vector2 gridPos = new Vector2(
				Mathf.Floor((player.transform.position.x-transform.position.x+gen.GetTileSize().x*(width+1)/2)*gen.GetTileSize().x),
				Mathf.Floor((player.transform.position.z-transform.position.z+gen.GetTileSize().y*(height+1)/2)*gen.GetTileSize().y));
			
			bool collision = false;
			for(int i = -1; i <= 1; i++){
				for(int j = -1; j <= 1; j++){
					if(gridPos.x + i < 0 || gridPos.y + j < 0 || gridPos.x + i >= width || gridPos.y + j >= height){
						continue;
					}
					if(grid[(int)((gridPos.x+i)+(gridPos.y+j)*width)] != null){
						if(grid[(int)((gridPos.x+i)+(gridPos.y+j)*width)].GetComponent<Tile>().Collide(player)){
							collision = true;
						}
					}
				}
			}
			return collision;
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
			t.Rotate(0f,0f,Mathf.Floor(Random.value*8)*90);
			t.parent = transform;
			t.GetComponent<Tile>().Init(t.position - Vector3.up*displacement, t.position, hideLowTiles);
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
			
			Gizmos.color = gizmoSolidColor;
			Gizmos.DrawCube(transform.position,
					new Vector3(tileSize.x * width, tileSize.z, tileSize.y * height));
			for(int i = 0; i < width*height; i++){
				Gizmos.DrawWireCube(transform.position + 
					new Vector3(tileSize.x*(i%width)-tileSize.x*(width-1)/2,0,tileSize.y*(int)(i/width)-tileSize.y*(height-1)/2),
						new Vector3(tileSize.x, tileSize.z, tileSize.y));
			}
		}
		private void OnDrawGizmosSelected() {
			if(gen == null){
				gen = GetGenerator();
			}
			if(tileSize.magnitude == 0) {
				tileSize = gen.GetTileSize();
			}
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
		private bool hide;
		
		// Variables:
		private List<Controller> controllers;
		private float jitter; // TODO rewrite jitter
		
		// Internals:
		internal void Init(Vector3 lowerPosition, Vector3 upperPosition, bool hide){
			this.lowerPosition = lowerPosition;
			this.upperPosition = upperPosition;
			this.hide = hide;
			
			this.controllers = new List<Controller>();
			this.jitter = 0.1f;
		}
		internal void AddController(Controller controller){
			if(controllers == null){
				controllers = new List<Controller>();
			}
			if(controller.GetInfluence(upperPosition) > 0 && !controllers.Contains(controller)){
				controllers.Add(controller);
			}
		}
		internal abstract bool Collide(Character.Player player);
		
		// Private functions:
		private void Update(){
			if(controllers == null){
				controllers = new List<Controller>();
			}
			
			// Sum up the influence of nearby controllers:
			float sum = 0;
			for(int i = 0; i < controllers.Count; i++){
				Controller controller = controllers[i];
				float inf = controller.GetInfluence(upperPosition);
				if(inf > 0 && controller.enabled == true) {
					sum += inf;
				} else {
					controllers.Remove(controller); // Remove controller if tile is outside it's influence
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
			if(hide && Vector3.Distance(transform.position,lowerPosition) < 0.0001f && controllers.Count == 0){
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
			if(prevPosition.x != transform.position.x || prevPosition.z != transform.position.z){
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Grid")){
					g.GetComponent<Grid>().AddController(this);
				}
			}
			prevPosition = transform.position;
		}
	}
}
