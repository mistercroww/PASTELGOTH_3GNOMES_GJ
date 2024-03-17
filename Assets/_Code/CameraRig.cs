using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {
    public Transform playerTarget;
    public float followSpeed;
    Transform t;
    private void Awake() {
        t = this.transform;
    }
    void FixedUpdate() {
        if (playerTarget == null) return;
        t.position = Vector3.Lerp(t.position, playerTarget.position, Time.deltaTime * followSpeed);
    }
}
