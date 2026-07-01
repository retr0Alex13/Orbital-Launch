using System.Collections.Generic;
using UnityEngine;

public sealed class AsteroidPool : MonoBehaviour
{
    private Asteroid prefab;
    private readonly Queue<Asteroid> free = new();

    public void Initialize(Asteroid asteroidPrefab, int size)
    {
        prefab = asteroidPrefab;
        Grow(size);
    }

    public Asteroid Get()
    {
        if (free.Count == 0)
        {
            Debug.LogWarning("[AsteroidPool] Pool exhausted — growing by 8. " +
                             "Increase poolSize in AsteroidRingConfig.");
            Grow(8);
        }
        return free.Dequeue();
    }

    public void Return(Asteroid a)
    {
        a.Deactivate();
        free.Enqueue(a);
    }

    private void Grow(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Asteroid a = Instantiate(prefab, transform);
            a.Deactivate();
            free.Enqueue(a);
        }
    }
}