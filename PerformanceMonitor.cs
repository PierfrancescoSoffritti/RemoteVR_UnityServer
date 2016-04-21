using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

    class PerformanceMonitor
    {
        private Timer mTimer;

        private List<int> history;
        private int counter;
        private double avg;

        public PerformanceMonitor()
        {
            mTimer = new Timer(1000);
            mTimer.Elapsed += OnTimedEvent;
            mTimer.AutoReset = true;

            history = new List<int>();
        }

        private void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            Debug.Log("\nPerformanceMonitor: " + counter + " image/second");
            history.Add(counter);
            counter = 0;
        }

        public void Start()
        {
            mTimer.Start();
        }

        public void Stop()
        {
            int sum = 0;
            foreach(int record in history)
                sum += record;

            avg = sum / history.Count;

            mTimer.Stop();
            mTimer.Dispose();

            Debug.Log("\nPerformanceMonitor AVG: " + avg + " image/second");
        }

        public void incCounter()
        {
            counter++;
        }
    }