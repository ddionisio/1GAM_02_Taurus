using UnityEngine;
using System.Collections;

public class CameraAttach : MonoBehaviour {
    public Transform attach;
    public bool lockX;
    public bool lockY;
    public bool lockZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        Vector3 attachPos = attach.position;

        if(attach != null && pos != attachPos) {
            if(!lockX) pos.x = attachPos.x;
            if(!lockY) pos.y = attachPos.y;
            if(!lockZ) pos.z = attachPos.z;
            transform.position = pos;
        }
	}
}
