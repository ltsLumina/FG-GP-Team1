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

    private void FixedUpdate()
    { 
        for (int i = 1; i < bodyParts.Count; i++)
        {
<<<<<<< Updated upstream
            Transform transform1 = bodyParts[i].transform;
            transform1.right = (bodyParts[i - 1].transform.position - transform1.position).normalized;
            if (Vector3.Distance(transform1.position, bodyParts[i - 1].transform.position) > jointLength)
            {
                transform1.position = Vector3.MoveTowards(transform1.position, bodyParts[i - 1].transform.position, moveSpeed * Time.deltaTime);
            }
=======
            var transform = bodyParts[i].transform;
            var posAhead = bodyParts[i-1].transform.position;
>>>>>>> Stashed changes

            transform.right = (posAhead - transform.position).normalized;
            if (Vector3.Distance(transform.position, posAhead) > jointLength)
            {
                transform.position = Vector3.MoveTowards(transform.position, posAhead, moveSpeed * Time.deltaTime);
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