using UnityEngine;
using System.Collections;

public class GODeactivateDelay : MonoBehaviour {
	public float delay;
	
	void OnEnable() {
		Invoke("OnDeactive", delay);
	}
	
	void OnDeactive() {
		gameObject.SetActive(false);
	}
}
