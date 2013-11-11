using UnityEngine;
using System.Collections;
using Grid;

[AddComponentMenu("Game/Tile/Normal")]
public class NormalTile : Tile  {
	private const float stepHeight = 1f;
	
	override internal bool Collide(Character.Player player){
		if(collider.bounds.Intersects(player.collider.bounds)){
			if(collider.bounds.max.y-player.collider.bounds.min.y < stepHeight){
				player.transform.Translate(0,collider.bounds.max.y-player.collider.bounds.min.y,0);
				return true;
			} else {
				// TODO wall collision
			}
		}
		return false;
	}
}
