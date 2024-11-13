using Lumina.Essentials.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EelModelMovement : MonoBehaviour
{ 

    [SerializeField] private List<Transform> bodyParts = new List<Transform>();
    [SerializeField] private float jointLength;
    [SerializeField] private float moveSpeed;

    private void Start()
    {


        foreach (Transform componentsInChild in GetComponentsInChildren<Transform>())
        {
            bodyParts.Add(componentsInChild);
        }

        bodyParts.RemoveAt(1);

        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].transform.position = new Vector3(transform.position.x + bodyParts.Count * jointLength, transform.position.y, transform.position.z);
            bodyParts[i].SetParent(null);
        }
    }

    private void Update()
    {

        for (int i = 1; i < bodyParts.Count; i++)
        {
            bodyParts[i].transform.right = (bodyParts[i - 1].transform.position - bodyParts[i].transform.position).normalized;
            if (Vector3.Distance(bodyParts[i].transform.position, bodyParts[i - 1].transform.position) > jointLength)
            {
                bodyParts[i].transform.position = Vector3.MoveTowards(bodyParts[i].transform.position, bodyParts[i - 1].transform.position, moveSpeed * Time.deltaTime);
            }

        }
    }

    private void OnDisable()
    {
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if (bodyParts[i] != null) Destroy(bodyParts[i].gameObject);
        }
        bodyParts.Clear();
    }

}