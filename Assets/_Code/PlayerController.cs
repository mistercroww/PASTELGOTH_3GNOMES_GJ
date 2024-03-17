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
    public float currentHealth;
    public float attackRange = 2f;
    public float attackAngleThresold = 0.33f;
    public Animator anim;
    Transform t, cam;
    bool isMoving, wasDamaged, atk_melee, atk_Click, incomingAttackMissed, attackTrigger;
    [Space()]
    public StatInfo mod_health;
    public StatInfo mod_speed, mod_strenght, mod_atkSpd, mod_dodge;
    public RadarChartRenderer statsRenderer;
    Rigidbody rb;
    public Vector3 finalMoveDir;
    float NextAttackHardCooldown;

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
        tStats[4] = mod_dodge;
        return tStats;
    }
    private void Start() {
        GameManager.OnTick_2 += TickHandler_2;
        GameManager.OnTick_10 += TickHandler_10;
        GameManager.OnTick_20 += TickHandler_20;
        statsRenderer.GenerateMesh(GetStats());
        currentHealth = mod_health.statValue;
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
        movementSpeed = mod_speed.statValue;
        Vector3 tVel = mod_speed.statValue * Time.fixedDeltaTime * 50f * finalMoveDir.normalized;
        tVel.y = rb.velocity.y;
        rb.velocity = tVel;
    }
    private void AttackHandler() {
        if (!isAlive) return;
        if(Time.time > NextAttackHardCooldown) {
            attackTrigger = false;
        }
        if (Input.GetMouseButtonDown(0)) {
            //ataca melee
            if (!attackTrigger) {
                attackTrigger = true;
                atk_Click = true;
                if (anim)anim.SetTrigger("Attack");

                NextAttackHardCooldown = Time.time + 1f;
                EnemyController[] enemies = FindObjectsOfType<EnemyController>();
                if (enemies != null) {
                    for (int i = 0; i < enemies.Length; i++) {
                        float dot = Vector3.Dot(t.forward, (enemies[i].transform.position - t.position));
                        if (dot > attackAngleThresold) {
                            if (Vector3.Distance(enemies[i].transform.position, t.position) <= attackRange) {
                                //atacar
                                enemies[i].GetDamage(GetModAttack());
                                atk_melee = true;
                            }
                        }
                    }
                }
            }
        }
    }
    private float GetModAttack() {
        return 10f + mod_strenght.statValue;
    }
    public void GetDamage(float dmgPoints) {
        float randAtkChance = UnityEngine.Random.value;
        if (randAtkChance > mod_dodge.statValue / 100f) {
            currentHealth -= UnityEngine.Random.Range(dmgPoints * 0.8f, dmgPoints * 1.2f);
            if (currentHealth <= 0) {
                Death();
            }
        }
        else {
            incomingAttackMissed = true;
        }
    }
    public void Death() {
        //player dies
        currentHealth = 0;
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
        //atack click
        if (atk_Click) {
            atk_Click = false;
            mod_atkSpd.statValue += mod_atkSpd.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Attack Speed", Color.green);
        }
        else {
            mod_atkSpd.statValue -= mod_atkSpd.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>--<size=100%>" + " Attack Speed", Color.red);
        }
        mod_atkSpd.statValue = Mathf.Clamp(mod_atkSpd.statValue, mod_atkSpd.statRange.x, mod_atkSpd.statRange.y);

        //atack STRENGHT
        if (atk_melee) {
            atk_melee = false;
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
        if (incomingAttackMissed) {
            incomingAttackMissed = false;
            mod_dodge.statValue += mod_dodge.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Dodge", Color.green);
        }
        else {
            mod_dodge.statValue -= mod_dodge.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>--<size=100%>" + " Dodge", Color.red);
        }
        mod_dodge.statValue = Mathf.Clamp(mod_dodge.statValue, mod_dodge.statRange.x, mod_dodge.statRange.y);

        //health stuff
        if (wasDamaged) {
            wasDamaged = false;
            mod_health.statValue += mod_health.gainRate;
            UI_Manager.instance.ShowPlayerText("<size=130%>++<size=100%>" + " Health", Color.green);
        }
        else {
            mod_health.statValue -= mod_health.gainRate;
            if(currentHealth > mod_health.statValue) {
                currentHealth = mod_health.statValue;
            }
            UI_Manager.instance.ShowPlayerText("<size=130%>--<size=100%>" + " Health", Color.red);
        }
        mod_health.statValue = Mathf.Clamp(mod_health.statValue, mod_health.statRange.x, mod_health.statRange.y);
    }
}
