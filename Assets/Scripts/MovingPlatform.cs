using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
	
	float startPos;
	float endPos;

	public int unitsToMove = 5;

	public int moveSpeed = 2;

	bool moveRight = true;
	

	void Awake(){
		startPos = transform.position.y;
		endPos = startPos + unitsToMove;
	}

	void Update(){
		if (moveRight) {
			transform.position += Vector3.up * moveSpeed * Time.deltaTime;	
		}
		if (transform.position.y >= endPos) {
			moveRight = false;
		}
		if (moveRight==false) {
			transform.position -= Vector3.up * moveSpeed * Time.deltaTime;	
		}
		if (transform.position.y <= startPos) {
			moveRight = true;
		}
	}
}