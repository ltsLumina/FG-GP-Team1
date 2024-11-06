using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelBody : MonoBehaviour
{
    [SerializeField] private int bodyPartAmount;
    private List<GameObject> bodyParts = new List<GameObject>();
    [SerializeField] Mesh mesh;
    [SerializeField] private float radius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Material material;

    private void Start()
    {
        bodyParts.Add(gameObject);

        for (int i = 0; i < bodyPartAmount; i++)
        {
            GameObject bodyPart = new GameObject("BodyPart");

            bodyPart.AddComponent<MeshFilter>().mesh = mesh;
            bodyPart.AddComponent<MeshRenderer>().material = material;

            bodyParts.Add(bodyPart);
            bodyPart.transform.position = new Vector3(transform.position.x + i * radius, transform.position.y, transform.position.z);
        }
    }

    private void Update()
    {

        for (int i = 1; i < bodyParts.Count; i++)
        {

            if (Vector3.Distance(bodyParts[i].transform.position, bodyParts[i - 1].transform.position) > radius)
            {
                bodyParts[i].transform.position = Vector3.MoveTowards(bodyParts[i].transform.position, bodyParts[i - 1].transform.position, moveSpeed * Time.deltaTime);
            }

        }
    }

}