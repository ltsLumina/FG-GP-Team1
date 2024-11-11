using UnityEngine;

public class KelpWaveAndRotate : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveSpeed = 1f;
    public float waveAmplitude = 2f;
    public Vector3 waveDirection = Vector3.forward;
    public float phaseOffset = 0f;        // Phase offset for waving

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public Vector3 rotationAxis = Vector3.up;

    private Vector3 initialRotation;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
        phaseOffset = Random.Range(1, 10);
    }

    void Update()
    {
        // Waving Motion with Phase Offset
        float waveAngle = waveAmplitude * Mathf.Sin(Time.time * waveSpeed + phaseOffset);
        Vector3 newWaveRotation = initialRotation + Vector3.Scale(waveDirection.normalized, new Vector3(waveAngle, waveAngle, waveAngle));
        transform.localEulerAngles = newWaveRotation;

        // Rotation Motion
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
