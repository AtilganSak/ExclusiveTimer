using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExclusiveTime;
using System;

public class CountDownTimerTest : MonoBehaviour
{
    public TMP_Text fullTimeStrText;

    public int second;

    ExTime countDownTimer;

    private void OnEnable()
    {
        countDownTimer = new ExTime(UsageType.CountDownTimer);
        //countDownTimer.SetCountDownTimer(59, 5, 2, 1);
        countDownTimer.SetCountDownTimer(second);
    }

    void Start()
    {       
        countDownTimer.StartTime();
    }
    
    void Update()
    {
        fullTimeStrText.text = countDownTimer.strSingleSecond;        
    }
}
