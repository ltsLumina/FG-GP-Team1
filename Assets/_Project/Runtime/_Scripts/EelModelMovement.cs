using System.Collections.Generic;
using UnityEngine;

public class EelModelMovement : MonoBehaviour
{ 

    [SerializeField] private List<Transform> bodyParts = new List<Transform>();
    [SerializeField] private float jointLength;
    [SerializeField] private float moveSpeed;

    static GameObject container;
    
    private void Start()
    {
        container = GameObject.Find("Eel Container");
        if (container == null) container = new GameObject("Eel Container");
        var parent = new GameObject("Eel");
        parent.transform.SetParent(container.transform);
        transform.SetParent(parent.transform);
        
        foreach (Transform componentsInChild in GetComponentsInChildren<Transform>())
        {
            bodyParts.Add(componentsInChild);
        }

        bodyParts.RemoveAt(1);

        foreach (Transform bodyPart in bodyParts)
        {
            bodyPart.transform.position = new Vector3(transform.position.x + bodyParts.Count * jointLength, transform.position.y, transform.position.z);
            bodyPart.SetParent(parent.transform);
        }
    }

    private void Update()
    {

        for (int i = 1; i < bodyParts.Count; i++)
        {
            Transform transform1 = bodyParts[i].transform;
            transform1.right = (bodyParts[i - 1].transform.position - transform1.position).normalized;
            if (Vector3.Distance(transform1.position, bodyParts[i - 1].transform.position) > jointLength)
            {
                transform1.position = Vector3.MoveTowards(transform1.position, bodyParts[i - 1].transform.position, moveSpeed * Time.deltaTime);
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