using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveV2Gen : MonoBehaviour
{
    [SerializeField] float CameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - CameraSpeed * Time.deltaTime, transform.position.z);
    }
}
