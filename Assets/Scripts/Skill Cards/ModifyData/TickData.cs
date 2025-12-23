using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TickData
{
    public sealed class Stack
    {
        public int Count;
        public Queue<float> ExpiryTimes;
        public Stack(float? duration, int initialCount)
        {
            Count = initialCount;
            if (duration.HasValue)
            {
                ExpiryTimes = new Queue<float>(initialCount);
                float expiry = Time.time + duration.Value;
                for (int i = 0; i < initialCount; i++)
                    ExpiryTimes.Enqueue(expiry);
            }
        }
        public void Add(int amount, float? durationSeconds)
        {
            Count += amount;

            if (!durationSeconds.HasValue) return;
            
            ExpiryTimes ??= new Queue<float>(8);

            float expiry = Time.time + durationSeconds.Value;
            for (int i = 0; i < amount; i++)
                ExpiryTimes.Enqueue(expiry);
        }
        public int ExpireNow(float now)
        {
            if (ExpiryTimes == null) return 0;

            int expired = 0;
            while (ExpiryTimes.Count > 0 && ExpiryTimes.Peek() <= now)
            {
                ExpiryTimes.Dequeue();
                expired++;
            }

            if (expired > 0)
                Count = Mathf.Max(0, Count - expired);

            return expired;
        }
    }
    public IDamageable damageable;
    public IUsesStatusEffects statusEffects;
    public readonly Dictionary<SkillContext, Stack> stacks = new();
    public float tickInterval;
    public float timer;
    public HitData hitData;
    public float durationElapsed;
    public int TotalStacks => stacks.Values.Sum(stack => stack.Count);
    public TickData(IDamageable damageable, IUsesStatusEffects statusEffects, TowerShooting tower, HitData hitData, float tickInterval)
    {
        this.damageable = damageable;
        this.statusEffects = statusEffects;
        this.tickInterval = tickInterval;
        this.hitData = hitData;
    }
    
}
