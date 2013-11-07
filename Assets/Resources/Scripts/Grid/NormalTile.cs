using UnityEngine;
using System.Collections;
using Grid;

[AddComponentMenu("Game/Tile/Normal")]
public class NormalTile : Tile  {
	
	override internal bool Collide(Character.Player player){
		if(collider.bounds.Intersects(player.collider.bounds)){
			player.transform.Translate(0,collider.bounds.max.y-player.collider.bounds.min.y,0);
		}
		return false;
	}
}
