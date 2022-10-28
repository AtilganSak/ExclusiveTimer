using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExclusiveTime;
using UnityEditor;

public class TestTimer : MonoBehaviour
{
    public TMP_Text secondText;
    public TMP_Text minuteText;
    public TMP_Text hourText;

    public TMP_Text fullTimeText;

    ExTime time;

    private void OnEnable()
    {
        time = new ExTime();
        time.Schedule("Alarm1", TimeUnitType.splitSecond, 59,true);
        time.AddListener(TimeUnitType.splitSecond,Alarm1/*,Alarm2*/);

        EditorApplication.pauseStateChanged += StateChange;
    }

    private void OnDisable()
    {
        time.RemoveListener(TimeUnitType.splitSecond,Alarm1/*,Alarm2*/);
        time.StopTime();
    }

    void Start()
    {
        time.StartTime();
    }

    private void Update()
    {
        // Discrete for int
        //secondText.text = time.second.ToString();
        //minuteText.text = time.minute.ToString();
        //hourText.text = time.hour.ToString();

        // Discrete for string
        //secondText.text = time.strSecond;
        //minuteText.text = time.strMinute;
        //hourText.text = time.strHour;
        
        fullTimeText.text = time.strFullTime;
   
        if (Input.GetKeyDown("p"))
        {
            time.PauseTime();
        }

        if (Input.GetKeyDown("s"))
        {
            time.StopTime();
        }
        if (Input.GetKeyDown("f"))
        {
            time.StartTime();
        }
    }
    
    void StateChange(PauseState state)
    {
        switch (state)
        {
            case PauseState.Paused:
                time.PauseTime();
                break;
            case PauseState.Unpaused:
                time.StartTime();
                break;
            default:
                break;
        }
    }

    void Alarm1(ScheduleTime resultAlrm)
    {
        Debug.Log("Alarm Name: " + resultAlrm.scheduleName);
        Debug.Log("Alarm Type: " + resultAlrm.scheduleType);
        Debug.Log("Alarm Time: " + resultAlrm.scheduleTime);
    }
    void Alarm2(ScheduleTime resultAlrm)
    {
        Debug.Log("Alarm2 Name: " + resultAlrm.scheduleName);
        Debug.Log("Alarm2 Type: " + resultAlrm.scheduleType);
        Debug.Log("Alarm2 Time: " + resultAlrm.scheduleTime);
    }
}
