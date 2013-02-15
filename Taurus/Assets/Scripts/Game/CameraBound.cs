using UnityEngine;
using System.Collections;

public class CameraBound : MonoBehaviour {

    public float width;
    public float height;

    public Transform cameraTransform; //the transform you want to set the position
    public Camera cameraAttach; //the camera you want to compare within cameraTransform (Make sure it is ortho)

    private Vector3 mCurCameraPos = Vector3.zero;

    public Vector3 Cap(Vector3 center, float halfW, float halfH, bool wrap = false) {
        Vector3 pos = center;

        float hWorldW = width * 0.5f;
        float hWorldH = height * 0.5f;

        Vector3 wPos = transform.position;
        wPos.x -= hWorldW * 0.5f;
        wPos.y -= hWorldH * 0.5f;

        if(pos.x - halfW < wPos.x) {
            if(wrap) {
                pos.x = wPos.x + hWorldW - halfW;
            }
            else {
                pos.x = wPos.x + halfW;
            }
        }
        else if(pos.x + halfW > wPos.x + hWorldW) {
            if(wrap) {
                pos.x = wPos.x + halfW;
            }
            else {
                pos.x = wPos.x + hWorldW - halfW;
            }
        }

        if(pos.y - halfH < wPos.y) {
            pos.y = wPos.y + halfH;
        }
        else if(pos.y + halfH > wPos.y + hWorldH) {
            pos.y = wPos.y + hWorldH - halfH;
        }

        return pos;
    }

    public Vector2 RandomLocation(float halfW, float halfH) {
        float hWorldW = width * 0.5f;
        float hWorldH = height * 0.5f;

        Vector2 ret = transform.position;
        ret.x -= hWorldW * 0.5f;
        ret.y -= hWorldH * 0.5f;

        ret.x = Random.Range(ret.x + halfW, ret.x + hWorldW - halfW);
        ret.y = Random.Range(ret.y + halfH, ret.y + hWorldH - halfH);

        return ret;
    }

    void Update() {
        if(cameraTransform != null && mCurCameraPos != cameraTransform.position) {
            float wRatio = cameraAttach.pixelWidth / cameraAttach.pixelHeight;

            mCurCameraPos = Cap(cameraTransform.position, cameraAttach.orthographicSize * wRatio, cameraAttach.orthographicSize);
            cameraTransform.position = mCurCameraPos;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width * 0.5f, height * 0.5f, 0.1f));
    }
}
