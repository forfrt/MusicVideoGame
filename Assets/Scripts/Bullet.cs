using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	int damageValue = 1;

	//用于碰撞时摧毁两个物体
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Enemy") {
			Destroy(gameObject);
			other.gameObject.SendMessage("EnemyDamaged",damageValue,SendMessageOptions.DontRequireReceiver);
		}

		if (other.gameObject.tag == "LevelObjects") {
			Destroy(gameObject);
		}
	}

	void FixedUpdate(){
		Destroy (gameObject, 1.25f);
	}
}
