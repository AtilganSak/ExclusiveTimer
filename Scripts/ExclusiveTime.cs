using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Timers;
using UnityEngine.Jobs;
using System;

namespace ExclusiveTime
{    
    public class ExTime
    {
        /// <summary>
        /// Return back to -Hour- unit a int format.
        /// </summary>
        public int hour { get; private set; }
        /// <summary>
        /// Return back to -Minute- unit a int format.
        /// </summary>
        public int minute { get; private set; }
        /// <summary>
        /// Return back to -second- unit a int format.
        /// </summary>
        public int second { get; private set; }
        /// <summary>
        /// Return back to -split-second- unit a int format.
        /// </summary>
        public int milliSecond { get; private set; }
        /// <summary>
        /// Return back to all time in the second unit a int format.
        /// </summary>
        public int singleSecond { get; private set; }

        /// <summary>
        /// Return back to -split-Second- unit a string format.
        /// </summary>
        public string strMilliSecond
        {
            get
            {
                if (milliSecond < 10)
                    return "0" + milliSecond;
                else
                    return milliSecond.ToString();
            }
        }
        /// <summary>
        /// Return back to -Second- unit a string format.
        /// </summary>
        public string strSecond
        {
            get
            {
                if (second < 10)
                    return "0" + second;
                else
                    return second.ToString();
            }
        }
        /// <summary>
        /// Return back to -Minute- unit a string format.
        /// </summary>
        public string strMinute
        {
            get
            {
                if (minute < 10)
                    return "0" + minute;
                else
                    return minute.ToString();
            }
        }
        /// <summary>
        /// Return back to -Hour- unit a string format.
        /// </summary>
        public string strHour
        {
            get
            {
                if (hour < 10)
                    return "0" + hour;
                else
                    return hour.ToString();
            }
        }
        /// <summary>
        /// Return back to all time in the second unit a string format.
        /// </summary>
        public string strSingleSecond {
            get{
                return singleSecond.ToString();
            }
        }
        /// <summary>
        /// All units return back to a single string.
        /// <para> "Example: return "01:30:15:43"; </para>        
        /// </summary>
        public string strFullTime
        {
            get
            {
                return strHour + ":" + strMinute + ":" + strSecond + ":" + "<size= 20>"+ strMilliSecond + "</size>";
            }
        }        

        private event UnityAction<ScheduleTime> scheduleSplitSecondEvent;
        private event UnityAction<ScheduleTime> scheduleSecondEvent;
        private event UnityAction<ScheduleTime> scheduleMinuteEvent;
        private event UnityAction<ScheduleTime> scheduleHourEvent;

        private ScheduleTime currentScheduleTime;

        private UsageType usageType;
        
        bool isScheduleForHour;
        bool isScheduleForMinute;
        bool isScheduleForSecond;
        bool isScheduleForSplitSecond;
        bool isSettedUpCountDownTimer;

        bool dontDestroyScehedule;

        bool isPause;
        bool isStop;
        bool isStart;

        Timer timer;       

        public ExTime(UsageType _usageType = UsageType.Timer)
        {
            usageType = _usageType;
            if(usageType == UsageType.CountDownTimer)
            {
                timer = new Timer(17);
                timer.Elapsed += CountDownTimer;
            }
            else if (usageType == UsageType.Timer)
            {
                timer = new Timer(17);
                timer.Elapsed += Timer;
            }             
        }

        /// <summary>
        /// Sets any time for events.
        /// </summary>
        /// <param name="UniqueName"></param>
        /// <param name="unitType"></param>
        /// <param name="time"></param>
        public void Schedule(string UniqueName, TimeUnitType unitType, int time, bool dontDestroyScehedule = false)
        {
            if (!currentScheduleTime.isFull && currentScheduleTime.scheduleName != UniqueName)
            {
                switch (unitType)
                {
                    case TimeUnitType.hour:
                        if (time > 24)
                        {
                            Debug.LogError("-Hour Time- cannot be greater than 24!");
                            return;
                        }
                        isScheduleForHour = true;                                                    
                        break;
                    case TimeUnitType.minute:
                        if (time > 60)
                        {
                            Debug.LogError("-Minute Time- cannot be greater than 60!");
                            return;
                        }
                        isScheduleForMinute = true;
                        break;
                    case TimeUnitType.second:
                        if (time > 60)
                        {
                            Debug.LogError("-Second Time- cannot be greater than 60!");
                            return;
                        }
                        isScheduleForSecond = true;
                        break;
                    case TimeUnitType.splitSecond:
                        if (time > 60)
                        {
                            Debug.LogError("-SplitSecond Time- cannot be greater than 60!");
                            return;
                        }
                        isScheduleForSplitSecond = true;
                        break;
                    default:
                        break;
                }

                this.dontDestroyScehedule = dontDestroyScehedule;

                currentScheduleTime.scheduleName = UniqueName;
                currentScheduleTime.scheduleType = unitType;
                currentScheduleTime.scheduleTime = time;
                currentScheduleTime.isFull = true;
            }
            else
            {
                Debug.LogWarning("Already have been one schedule!");
            }
        }
        /// <summary>
        /// Set time parameter of CountDownTimer.
        /// </summary>
        /// <param name="_splitSecond"></param>
        /// <param name="_second"></param>
        /// <param name="_minute"></param>
        /// <param name="_hour"></param>
        public void SetCountDownTimer(int _splitSecond, int _second, int _minute, int _hour)
        {
            if (usageType != UsageType.CountDownTimer) return;

            if (_splitSecond >= 0 && _splitSecond < 60)
                milliSecond = _splitSecond;
            if (_second >= 0 && _second < 60)
                second = _second;
            if (_minute >= 0 && _minute < 60)
                minute = _minute;
            if (_hour >= 0 && _hour <= 24)
                hour = _hour;

            isSettedUpCountDownTimer = true;
        }
        /// <summary>
        /// Set time parameter of CountDownTimer for one Second time.
        /// </summary>
        /// <param name="_singleSecond"></param>
        public void SetCountDownTimer(int _singleSecond)
        {
            if (usageType != UsageType.CountDownTimer) return;

            singleSecond = _singleSecond;

            TimeSpan sngSecond = TimeSpan.FromSeconds(_singleSecond);

            milliSecond = sngSecond.Milliseconds;
            second = sngSecond.Seconds;
            minute = sngSecond.Minutes;
            hour = sngSecond.Hours;

            isSettedUpCountDownTimer = true;
        }

        #region SSP Process               
        public void StartTime()
        {
            if ((!isStop || !isPause) && isStart) return;
            if (usageType == UsageType.CountDownTimer)           
                if (!isSettedUpCountDownTimer) throw new System.Exception("Not Setted Up CountDownTimer!");                                                

            isStart = true;            
            timer.Start();
            if (isStop)
                isStop = false;
            else if (isPause)            
                isPause = false;                        
        }
        public void StopTime()
        {
            timer.Stop();
            isStop = true;
            isStart = false;
            Reset();
        }
        public void PauseTime()
        {
            if (isStop) return;

            timer.Stop();
            isStart = false;
            isPause = true;
        }
        #endregion

        #region Event Process        
        /// <summary>
        /// Add listener to specific event;
        /// </summary>
        /// <param name="method"></param>
        /// <param name="type"></param>
        public void AddListener(TimeUnitType type, params UnityAction<ScheduleTime>[] method)
        {
            switch (type)
            {
                case TimeUnitType.hour:
                    for (int i = 0; i < method.Length; i++)                    
                        scheduleHourEvent += method[i];
                    break;
                case TimeUnitType.minute:
                    for (int i = 0; i < method.Length; i++)
                        scheduleMinuteEvent += method[i];                    
                    break;
                case TimeUnitType.second:
                    for (int i = 0; i < method.Length; i++)
                        scheduleSecondEvent += method[i];                    
                    break;
                case TimeUnitType.splitSecond:
                    for (int i = 0; i < method.Length; i++)
                        scheduleSplitSecondEvent += method[i];                    
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Remove listener to specific event;
        /// </summary>
        /// <param name="method"></param>
        /// <param name="type"></param>
        public void RemoveListener(TimeUnitType type, params UnityAction<ScheduleTime>[] method)
        {           
            switch (type)
            {
                case TimeUnitType.hour:
                    for (int i = 0; i < method.Length; i++)                        
                        scheduleHourEvent -= method[i];
                    break;
                case TimeUnitType.minute:
                    for (int i = 0; i < method.Length; i++)
                        scheduleMinuteEvent -= method[i];
                    break;
                case TimeUnitType.second:
                    for (int i = 0; i < method.Length; i++)
                        scheduleSecondEvent -= method[i];
                    break;
                case TimeUnitType.splitSecond:
                    for (int i = 0; i < method.Length; i++)
                        scheduleSplitSecondEvent -= method[i];
                    break;
                default:
                    break;
            }        
        }
        #endregion

        #region Time Process        
        private void IncSplitSecond()
        {
            milliSecond++;           

            CheckSchedule(TimeUnitType.splitSecond, milliSecond);

            if (milliSecond >= 60)
            {
                milliSecond = 0;
                IncSecond();
            }
        }
        private void IncSecond()
        {
            second++;
            singleSecond++;

            CheckSchedule(TimeUnitType.second, second);

            if (second >= 60)
            {
                second = 0;
                IncMinute();
            }
        }
        private void IncMinute()
        {
            minute++;

            CheckSchedule(TimeUnitType.minute, minute);

            if (minute >= 60)
            {
                minute = 0;
                IncHour();
            }
        }
        private void IncHour()
        {
            hour++;

            CheckSchedule(TimeUnitType.hour, hour);

            if (minute >= 24)
            {
                hour = 0;
            }
        }
        #endregion
        #region Countdown Timer Process
        private void DecMilliSecond()
        {
            if(milliSecond > 0)
                milliSecond--;            

            CheckSchedule(TimeUnitType.splitSecond, milliSecond);

            if (milliSecond == 0)
            {
                if (second > 0)
                {
                    milliSecond = 59;
                    DecSecond();
                }
                else if(minute > 0)
                {
                    milliSecond = 59;
                    second = 59;
                    DecMinute();
                }
                else if(hour > 0)
                {
                    milliSecond = 59;
                    second = 59;
                    minute = 59;
                    DecHour();
                }
                else
                {
                    StopTime();
                }
            }            
        }
        private void DecSecond()
        {
            if(second > 0)
                second--;
            if (singleSecond > 0)
                singleSecond--;

            CheckSchedule(TimeUnitType.second, second);

            if (second == 0)
            {
                if(minute > 0)
                {
                    second = 59;                  
                    DecMinute();
                }              
            }
        }
        private void DecMinute()
        {
            if (minute > 0)
                minute--;

            CheckSchedule(TimeUnitType.minute, minute);

            if (minute == 0)
            {
                if (hour > 0)
                {
                    minute = 59;                    
                    DecHour();
                }            
            }
        }
        private void DecHour()
        {
            if (hour > 0)
                hour--;           

            CheckSchedule(TimeUnitType.hour, hour);           
        }
        #endregion

        private void Reset()
        {
            hour = 0;
            minute = 0;
            second = 0;
            milliSecond = 0;
            singleSecond = 0;
        }

        private void CheckSchedule(TimeUnitType type, int currentTime)
        {        
            switch (type)
            {
                case TimeUnitType.hour:
                    if (!isScheduleForHour) return;
                    if (currentScheduleTime.scheduleTime <= 24)
                        if (currentScheduleTime.scheduleTime == currentTime)
                            if (scheduleHourEvent != null)
                            {
                                scheduleHourEvent.Invoke(currentScheduleTime);                                
                                if (!dontDestroyScehedule)
                                    isScheduleForHour = false;                                
                            }
                    break;
                case TimeUnitType.minute:
                    if (!isScheduleForMinute) return;
                    if (currentScheduleTime.scheduleTime <= 60)
                        if (currentScheduleTime.scheduleTime == currentTime)
                            if (scheduleMinuteEvent != null)
                            {
                                scheduleMinuteEvent.Invoke(currentScheduleTime);
                                if (!dontDestroyScehedule)
                                    isScheduleForMinute = false;
                            }
                    break;
                case TimeUnitType.second:
                    if (!isScheduleForSecond) return;
                    if (currentScheduleTime.scheduleTime <= 60)
                        if (currentScheduleTime.scheduleTime == currentTime)
                            if (scheduleSecondEvent != null)
                            {
                                scheduleSecondEvent.Invoke(currentScheduleTime);
                                if (!dontDestroyScehedule)
                                    isScheduleForSecond = false;
                            }
                    break;
                case TimeUnitType.splitSecond:
                    if (!isScheduleForSplitSecond) return;
                    if (currentScheduleTime.scheduleTime <= 60)
                        if (currentScheduleTime.scheduleTime == currentTime)
                            if (scheduleSplitSecondEvent != null)
                            {
                                scheduleSplitSecondEvent.Invoke(currentScheduleTime);
                                if (!dontDestroyScehedule)
                                    isScheduleForSplitSecond = false;
                            }
                    break;
                default:
                    break;
            }
            if (!dontDestroyScehedule)
                currentScheduleTime.Reset();
        }

        private void Timer(object o, ElapsedEventArgs a)
        {            
            IncSplitSecond();            
        }
        private void CountDownTimer(object o, ElapsedEventArgs a)
        {
            DecMilliSecond();
        }
    }   
    public struct ScheduleTime
    {
        public bool isFull;

        public string scheduleName;
        public TimeUnitType scheduleType;
        public int scheduleTime;

        public void Reset()
        {
            isFull = false;

            scheduleName = "";
            scheduleType = TimeUnitType.none;
            scheduleTime = 0;
        }
    }
    public enum UsageType { Timer, CountDownTimer }
    public enum TimeUnitType { hour, minute, second, splitSecond, none }
}