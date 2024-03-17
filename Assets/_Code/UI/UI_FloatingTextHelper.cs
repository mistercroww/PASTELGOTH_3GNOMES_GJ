using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_FloatingTextHelper : MonoBehaviour
{
    public TMP_Text text;
    public float moveSpeed = 1f;
    public AnimationCurve alphaCurve;
    public float alphaCurveDuration = 2f;
    float lerp;
    Color cachedColor;
    Transform t;
    private void Awake() {
        t = this.transform;
    }
    private void Update() {
        lerp += Time.deltaTime / alphaCurveDuration;
        text.color = Color.Lerp(Color.clear, cachedColor, alphaCurve.Evaluate(lerp));
        t.localPosition += moveSpeed * Time.deltaTime * Vector3.up;
        if(lerp >= 1f) {
            Destroy(gameObject);
        }
    }
    public void SetupFloatingText(string _text, Color color) {
        text.text = _text;
        text.color = color;
        cachedColor = color;
    }
}
