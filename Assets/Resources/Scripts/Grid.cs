using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	private Color gizmoSolidColor = Color.white * 0.5f;
	private Color gizmoWireColor = Color.white * 0.7f;
	
	public int width = 1;
	public int height = 1;
	public Transform tile = null;
	
	private Vector3 tileSize;
	private float[] grid;
	
	void Start () {
		if(width <= 0 || height <= 0){
			throw(new UnityException("Grid: Invalid size."));
		}
		if(tile == null){
			throw(new UnityException("Grid: No tile selected."));
		}
		
		tileSize = new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
			tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
			tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
		tile.rotation = Quaternion.identity;
		
		grid = new float[width * height];
		int i = grid.Length;
		Transform t;
		while(i-- > 0){
			t = Instantiate(tile, transform.position + new Vector3(
				(i%width - ((float)width/2) + 0.5f) * tileSize.x,0,
				((int)(i/width) - ((float)height/2) + 0.5f) * tileSize.z), 
				Quaternion.identity) as Transform;
			t.parent = transform;
		}
	}
	
	void Update () {
	
	}
	
	void OnDrawGizmosSelected() {
		if(tileSize.magnitude == 0) {
			tileSize = new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
				tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
				tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
		}
		Gizmos.color = gizmoWireColor;
		Gizmos.DrawWireCube(transform.position,new Vector3(tileSize.x * width, tileSize.y, tileSize.z * height));
	}
	
	void OnDrawGizmos() {
		if(tileSize.magnitude == 0) {
			tileSize = new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
				tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
				tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
		}
		Gizmos.color = gizmoSolidColor;
		Gizmos.DrawCube(transform.position,new Vector3(tileSize.x * width, tileSize.y, tileSize.z * height));
	}
}
