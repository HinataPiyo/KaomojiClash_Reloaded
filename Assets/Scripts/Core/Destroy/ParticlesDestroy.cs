using UnityEngine;

public class ParticlesDestroy : MonoBehaviour
{
    ParticleSystem[] ps;

    void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (ps.Length == 0) return;

        bool allStopped = true;
        foreach (var particle in ps)
        {
            if (particle.isPlaying)
            {
                allStopped = false;
                break;
            }
        }

        if (allStopped)
        {
            Destroy(gameObject);
        }
    }
}