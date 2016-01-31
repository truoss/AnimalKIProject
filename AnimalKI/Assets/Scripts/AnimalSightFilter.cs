using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalSightFilter
{
    public float lastChangedThreshold = 5;
    public float MemoryTime = 60;
    Hashtable memory = new Hashtable();

    public AnimalSightFilter(float lastChangedThreshold, float memoryTime)
    {
        this.lastChangedThreshold = lastChangedThreshold;
        MemoryTime = memoryTime;
    }

    //List<Transform> opti = new List<Transform>();
    //List<Transform> toRemove = new List<Transform>();

    public void ClearMemory()
    {
        memory.Clear();
    }

    public Transform[] OptimizedSightings(Transform[] rawSightings)
    {
        //update memory
        for (int i = 0; i < rawSightings.Length; i++)
        {
            if (memory.Contains(rawSightings[i]))
            {
                var rigid = rawSightings[i].GetComponent<Rigidbody>();

                if (rigid && rigid.velocity != Vector3.zero)
                    memory[rawSightings[i]] = Time.time;
                else if ((float)memory[rawSightings[i]] + MemoryTime < Time.time)
                    memory.Remove(rawSightings[i]);
            }
            else
                memory.Add(rawSightings[i], Time.time);
        }

        List<Transform> tmp = new List<Transform>();
        int n = 0;
        foreach (Transform key in memory.Keys)
        {
            if (!((float)memory[key] + lastChangedThreshold < Time.time))
                tmp.Add(key);
        }

        return tmp.ToArray();
    }
}
