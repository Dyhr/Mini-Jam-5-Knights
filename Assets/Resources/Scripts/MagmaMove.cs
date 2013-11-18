using UnityEngine;
using System.Collections;

public class MagmaMove : MonoBehaviour {
	
	public float texAngle = -45f;
	public float texSpeed = 0.01f;
	public float heightAngle = 90f;
	public float heightSpeed = 0.03f;
	
	private Vector2 texdir;
	private Vector2 heightdir;
	private string[] texs;
	
	private void Start() {
		texdir = new Vector2(Mathf.Cos(texAngle)*texSpeed,Mathf.Sin(texAngle)*texSpeed);
		heightdir = new Vector2(Mathf.Cos(heightAngle)*heightSpeed,Mathf.Sin(heightAngle)*heightSpeed);
		texs = new string[]{
			"_BumpMap", "_ParallaxMap"
		};
	}
	
	private void Update () {
		foreach(string s in texs){
			Vector2 offset = renderer.material.GetTextureOffset(s);
			renderer.material.SetTextureOffset(s,offset+heightdir*(Time.deltaTime));
		}
		renderer.material.mainTextureOffset += texdir*(Time.deltaTime);
	}
}
