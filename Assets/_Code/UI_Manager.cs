using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public struct PlayerPopUpText {
    public string s;
    public Color c;
}
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;
    public Transform floatingTextsParent;
    public UI_FloatingTextHelper playerFloatingTextPrefab;
    public Queue<PlayerPopUpText> popupsQueue = new();
    float NextQueueCheck;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        GameManager.OnTick += CheckQueue;
    }
    private void OnDestroy() {
        GameManager.OnTick -= CheckQueue;
    }
    /*
    private void Update() {
        if(Time.time > NextQueueCheck) {
            NextQueueCheck = Time.time + 0.5f;
            CheckQueue();
        }
    }
    */
    private void CheckQueue(int curTick) {
        if (popupsQueue.Count > 0) {
            var tPopup = popupsQueue.Dequeue();
            var tText = Instantiate(playerFloatingTextPrefab, floatingTextsParent);
            tText.SetupFloatingText(tPopup.s, tPopup.c);
        }
    }
    public void ShowPlayerText(string s, Color c) {
        var tPopup = new PlayerPopUpText {
            s = s,
            c = c
        };
        popupsQueue.Enqueue(tPopup);

    }
}
