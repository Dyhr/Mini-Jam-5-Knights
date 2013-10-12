using UnityEngine;
using System.Collections;
using grid;

public class NormalPlatform : Controller {

	private void Start(){
		radius = 3;
		GameObject.Find("Grid").GetComponent<Grid>().AddController(this);
	}
	
	internal override float GetInfluence (Vector3 pos){
		return Mathf.Max(0,radius - Vector3.Distance(pos,transform.position));
	}	
}
