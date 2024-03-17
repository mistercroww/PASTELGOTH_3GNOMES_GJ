using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarChartRenderer : MonoBehaviour
{
    public float radarSize = 1f;
    public CanvasRenderer canvasRenderer;
    public Material mat;
    public Texture2D tex;
    public Gradient radarColorGradient;
    public float radarGradientDuration = 1f;
    float colorLerp;
    void Start() {
        //GenerateMesh();
    }
    private void Update() {
        colorLerp += Time.deltaTime / radarGradientDuration;
        if (colorLerp > 1f) colorLerp = 0;
        mat.SetColor("_Color", radarColorGradient.Evaluate(colorLerp));
    }
    public void GenerateMesh(StatInfo[] newStats) {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] tris = new int[15];

        float baseOffset = 5f;
        for (int i = 0; i < newStats.Length; i++) {
            vertices[i+1] = VertexPosition(i, newStats[i].statValue+ baseOffset, newStats.Length, newStats[i].statRange);
            uv[i + 1] = Vector2.one;
        }
        int currentIndex = 2;
        for (int i = 0; i < tris.Length; i++) {
            if (i % 3 == 0 || i == 0) {
                tris[i] = 0;
                currentIndex--;
            }
            else {
                tris[i] = currentIndex;
                currentIndex = tris[i]+1;
            }
            if(i == tris.Length - 1) {
                tris[i] = 1;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;
        canvasRenderer.SetMesh(mesh);
        canvasRenderer.SetMaterial(mat, tex);
    }
    private Vector3 VertexPosition(int index, float statValue, int statsCount, Vector2 range) {
        return Quaternion.Euler(0, 0, 
            -(360f / statsCount) * index) * Vector3.up * NormalizedStatToRange(statValue, range) * radarSize;
    }
    private float NormalizedStatToRange(float stat, Vector2 range) {
        return Mathf.InverseLerp(range.x, range.y, stat);
    }
}
