using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float currentHealth = 100f;
    public float dodgeValue = 10f;
    public void GetDamage(float dmgPoints) {
        float randAtkChance = Random.value;
        if (randAtkChance > dodgeValue / 100f) {
            currentHealth -= Random.Range(dmgPoints * 0.8f, dmgPoints * 1.2f);
            if (currentHealth <= 0) {
                Death();
            }
        }
    }
    public void Death() {
        //enemy dies
        currentHealth = 0;
    }
}
