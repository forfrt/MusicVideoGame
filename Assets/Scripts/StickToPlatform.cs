using UnityEngine;
using System.Collections;

public class StickToPlatform : MonoBehaviour {

	void OnTriggerStay(Collider other){
		if (other.tag == "Platform") {
			this.transform.parent = other.transform;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Platform") {
			this.transform.parent = null;
		}
	}
}
