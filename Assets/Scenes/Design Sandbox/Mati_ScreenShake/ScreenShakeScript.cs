using Unity.Cinemachine;
using UnityEngine;

public class ScreenShakeScript : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeStrength = 1.0f;
    [SerializeField] private float shakeCooldown = 0.2f;
    private float lastShakeTime;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            Debug.Log(this + " is missing CinemachineImpulseSource, please add it to components and retry");
            this.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impulseSource == null || Time.time - lastShakeTime < shakeCooldown) return;

        float shakeIntensity = shakeStrength;

        if (collision.gameObject.GetComponent<Train>())
        {
            impulseSource.GenerateImpulse(Vector3.one * shakeIntensity);
            lastShakeTime = Time.time; 
        }
        else if (collision.gameObject.GetComponent<Player>())
        {
            shakeIntensity *= 0.3f;
            impulseSource.GenerateImpulse(Vector3.one * shakeIntensity);
            lastShakeTime = Time.time; 
        }
    }
}
