using UnityEngine;

public class DeleteOffScreen : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (Mathf.Abs(cam.transform.position.x - transform.position.x) > 100 || Mathf.Abs(cam.transform.position.y - transform.position.y) > 100 || cam.transform.position.z - transform.position.z > 30)
        {
            Destroy(gameObject);
        }
    }

}
