using UnityEngine;

public class DirtinessParticleController : MonoBehaviour
{
    ParticleSystem system;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    public void EmissionRatePerStage(int stage)
    {
        ParticleSystem.EmissionModule systemEmission = system.emission;
        
        switch (stage)
        {
            case 1:
                Debug.Log("Stage 1");
                systemEmission.rateOverTime = 1f;
                break;
            
            case 2:
                Debug.Log("Stage 2");
                systemEmission.rateOverTime = 5f;
                break;
            
            case 3:
                Debug.Log("Stage 3");
                systemEmission.rateOverTime = 10f;
                break;
                
        }
    }
}
