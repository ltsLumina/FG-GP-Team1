using UnityEngine;

public class Scanner : MonoBehaviour
{
    // Target that the scanner will track (e.g., the player)
    public Transform target;

    // Speed of rotation
    [SerializeField]
    float rotationSpeed = 2.0f;

    [SerializeField]
    GameObject scannerLight;

    //Inital Rotation
    private Quaternion initialRotation;

    bool isScanning = false;

    void Awake()
    {
        // TODO Subscribe to scanner events in GameManager
    }

    void Start()
    {
        // Set initial rotation
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (isScanning)
        {
            Scan();
        }
    }

    void ResetScanner()
    {
        // Reset rotation
        isScanning = false;
        transform.rotation = Quaternion.identity;
        scannerLight.SetActive(false);
    }

    void Scan()
    {
        if (target != null)
        {
            // Calculate direction from scanner to target
            Vector3 directionToTarget = target.position - transform.position;

            // Calculate the desired rotation to look at the target
            Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Adjust for parent's initial rotation offset
            Quaternion rotationOffset = Quaternion.Euler(270, 0, 0); // Adjusted to face the other end of the camera
            desiredRotation *= rotationOffset;

            // Smoothly rotate the scanner towards the target, applying rotation correction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // No target
            ResetScanner();
        }
    }

    void OnScanStart()
    {
        isScanning = true;
        scannerLight.SetActive(true);
    }

    void OnScanStop()
    {
        ResetScanner();
    }
}
