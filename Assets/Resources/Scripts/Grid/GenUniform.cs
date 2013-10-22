using UnityEngine;
using System.Collections;
using Grid;

[AddComponentMenu("Game/Generator/One Type")]
public class GenUniform : Generator {

	public Transform tile;
	
	override public void Init(){
		if(tile == null){
			throw(new UnityException("Generator: No tile selected."));
		}
		if(tile.GetComponent<Tile>() == null){
			throw(new UnityException("Generator: Tile doesn't contain a tile script."));
		}
		if(tile.GetComponent<BoxCollider>() == null){
			throw(new UnityException("Generator: Tile doesn't contain a tile script."));
		}
	}
	override public Transform GetTile(Vector2 size, Vector2 pos){
		return tile;
	}
	override public Vector3 GetTileSize(){
		if(tile != null) {
			return new Vector3(tile.GetComponent<BoxCollider>().size.x * tile.lossyScale.x,
					tile.GetComponent<BoxCollider>().size.y * tile.lossyScale.y,
					tile.GetComponent<BoxCollider>().size.z * tile.lossyScale.z);
		} else {
			return Vector3.one;
		}
	}
}
