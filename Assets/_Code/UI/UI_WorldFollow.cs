using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WorldFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    Transform t;
    private void Awake() {
        t = this.transform;
    }
    void LateUpdate() {
        if (!target) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        t.position = screenPos + offset;
    }
}
