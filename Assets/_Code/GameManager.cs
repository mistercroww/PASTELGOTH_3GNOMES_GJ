using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action<int> OnTick;
    public static event Action<int> OnTick_2;
    public static event Action<int> OnTick_5;
    public static event Action<int> OnTick_10;
    public static event Action<int> OnTick_20;

    const float TICK_TIMER_MAX = 0.5f;
    int tick;
    float tickTimer;

    private void Awake() {
        instance = this;
        tick = 0;
    }
    private void Update() {
        tickTimer += Time.deltaTime;
        if (tickTimer >= TICK_TIMER_MAX) {
            tickTimer = 0;
            tick++;

            OnTick?.Invoke(tick);

            if(tick % 2 == 0) {
                OnTick_2?.Invoke(tick);
            }
            if (tick % 5 == 0) {
                OnTick_5?.Invoke(tick);
            }
            if (tick % 10 == 0) {
                OnTick_10?.Invoke(tick);
            }
            if (tick % 20 == 0) {
                OnTick_20?.Invoke(tick);
            }
        }
    }
}
