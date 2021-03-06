﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Font mariofont;

    public Text score_display;
    public Text coins_display;
    public Text timer_display;
    public Text gameover_display;

    private int score = 0;
    private int coins = 0;
    private int total_time = 999;
    //private int total_time = 101;
    //private int total_time = 2;

    // Start is called before the first frame update
    void Start()
    {
            mariofont.material.mainTexture.filterMode = FilterMode.Point;
            mariofont.material.mainTexture.anisoLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        update_display();

        if (total_time - Time.time <= 0)
        {
            ((PlayerMove)GameObject.Find("Player").GetComponent(typeof(PlayerMove))).die();
        }
    }

    void update_display()
    {
        score_display.text = score.ToString().PadLeft(6, "0"[0]);
        coins_display.text = coins.ToString().PadLeft(2, "0"[0]);
        timer_display.text = (Mathf.Max(0, (int)(total_time-Time.time))).ToString().PadLeft(3, "0"[0]);
    }

    public void add_score(int amount)
    {
        score += amount;
    }
    public void add_coins(int amount)
    {
        coins += amount;
        add_score(amount * 100);
    }

    public void game_over()
    {
        gameover_display.enabled = true;
    }
}
