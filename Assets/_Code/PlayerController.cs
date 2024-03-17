using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatInfo {
    public float statValue;
    public Vector2 statRange;
    public float gainRate;
}
public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    public bool isAlive = true;
    public float movementSpeed;
    Transform t, cam;
    bool isMoving, wasDamaged, atk_melee;
    [Space()]
    public StatInfo mod_health;
    public StatInfo mod_speed, mod_strenght, mod_atkSpd, mod_curse;
    public RadarChartRenderer statsRenderer;
    Rigidbody rb;
    public Vector3 finalMoveDir;

    private void Awake() {
        instance = this;
        t = this.transform;
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }
    public StatInfo[] GetStats() {
        var tStats = new StatInfo[5];
        tStats[0] = mod_health;
        tStats[1] = mod_speed;
        tStats[2] = mod_strenght;
        tStats[3] = mod_atkSpd;
        tStats[4] = mod_curse;
        return tStats;
    }
    private void Start() {
        GameManager.OnTick_2 += TickHandler_2;
        GameManager.OnTick_10 += TickHandler_10;
        GameManager.OnTick_20 += TickHandler_20;
        statsRenderer.GenerateMesh(GetStats());
    }

    private void OnDestroy() {
        GameManager.OnTick_2 -= TickHandler_2;
        GameManager.OnTick_10 -= TickHandler_10;
        GameManager.OnTick_20 -= TickHandler_20;
    }
    private void Update() {
        InputHandler();
        if (finalMoveDir != Vector3.zero) {
            isMoving = true;
            Quaternion tRot = Quaternion.LookRotation(finalMoveDir);
            t.rotation = Quaternion.Slerp(t.rotation, tRot, Time.deltaTime * 9f);
        }
        else {
            isMoving = false;
        }
        AttackHandler();
    }
    private void FixedUpdate() {
        MovementHandler();
    }
    private void InputHandler() {
        if (!isAlive) return;
        Vector2 inputDir = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        finalMoveDir = cam.right * inputDir.x + cam.forward * inputDir.y;
        finalMoveDir.y = 0;
    }
    private void MovementHandler() {
        if (!isAlive) return;
        //t.position += (movementSpeed + mod_speed.statValue) * Time.deltaTime * camRelativeDir.normalized;
        Vector3 tVel = (movementSpeed + mod_speed.statValue) * Time.fixedDeltaTime * 50f * finalMoveDir.normalized;
        tVel.y = rb.velocity.y;
        rb.velocity = tVel;
    }
    private void AttackHandler() {
        if (!isAlive) return;
        if (Input.GetMouseButtonDown(0)) {
            //ataca melee
        }
        /*
        else if (Input.GetMouseButtonDown(1)) {
            //ataca magico
        }
        */
    }
    private void TickHandler_2(int curTicks) {
        if (isMoving) {
            mod_speed.statValue += mod_speed.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Speed", Color.green);
        }
        else {
            mod_speed.statValue -= mod_speed.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=120%>--<size=100%>" + " Speed", Color.red);
        }
        mod_speed.statValue = Mathf.Clamp(mod_speed.statValue, mod_speed.statRange.x, mod_speed.statRange.y);
        //cada tick generamos de nuevo la mesh de STATS
        statsRenderer.GenerateMesh(GetStats());
    }
    private void TickHandler_10(int curTicks) {
        //atack melee
        if (atk_melee) {
            mod_strenght.statValue += mod_strenght.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Strenght", Color.green);
        }
        else {
            mod_strenght.statValue -= mod_strenght.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>--<size=100%>" + " Strenght", Color.red);
        }
        mod_strenght.statValue = Mathf.Clamp(mod_strenght.statValue, mod_strenght.statRange.x, mod_strenght.statRange.y);

    }
    private void TickHandler_20(int curTicks) {
        //health
        if (wasDamaged) {
            mod_health.statValue += mod_health.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Health", Color.green);
        }
        else {
            mod_health.statValue -= mod_health.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>--<size=100%>" + " Health", Color.red);
        }
        mod_health.statValue = Mathf.Clamp(mod_health.statValue, mod_health.statRange.x, mod_health.statRange.y);

    }
}
