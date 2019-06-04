using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	int damageValue = 1;

	//用于碰撞时摧毁两个物体
	void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Enemy") {

            print("current gameObject.position");
            print(gameObject.transform.position);

            gameObject.GetComponent<Rigidbody>().transform.position += new Vector3(10, 10, 0);
            print("changed gameObject.position");
            print(gameObject.transform.position);

            print("gameObject.GetComponent<Rigidbody>().velocity");
            print(gameObject.GetComponent<Rigidbody>().velocity);

            //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            
            Destroy(gameObject);
            other.gameObject.SendMessage("EnemyDamaged",damageValue,SendMessageOptions.DontRequireReceiver);
        }

        if (other.gameObject.tag == "LevelObjects") {
			Destroy(gameObject);
		}
	}

	void FixedUpdate(){
		Destroy (gameObject, 12.25f);
	}
}
