using System;
using System.Collections.Generic;

namespace Aiv.Engine
{
    public class TimerManager
    {
        private GameObject owner;
        private Dictionary<string, float> timers;
        private Dictionary<string, Action<GameObject>> callBacks;

        public TimerManager(GameObject owner)
        {
            this.owner = owner;

            timers = new Dictionary<string, float>();
            callBacks = new Dictionary<string, Action<GameObject>>();
        }

        public float Get(string key)
        {
            float result;
            if (!timers.TryGetValue(key, out result))
                return 0f;//float.MaxValue; // default value
            return result;
        }

        public void Set(string key, float value, Action<GameObject> callback = null)
        {
            timers[key] = value;
            if (callback != null)
                callBacks[key] = callback;
            else if (callBacks.ContainsKey(key) && callBacks[key] != null)
                callBacks[key] = null;
        }

        public void Update()
        {
            var keys = new string[timers.Count];
            timers.Keys.CopyTo(keys, 0);
            foreach (var key in keys)
            {
                if (timers[key] > 0f) { 
                    timers[key] -= owner.deltaTime;
                    if (callBacks.ContainsKey(key))
                        callBacks[key](owner);
                }
            }
        }
    }
}