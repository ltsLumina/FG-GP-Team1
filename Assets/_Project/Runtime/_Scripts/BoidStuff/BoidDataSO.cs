using UnityEngine;


[CreateAssetMenu(fileName = "New Boid Data", menuName = "New Boid Data", order = 1)]
public class BoidDataSO : ScriptableObject
{
    [Header("Movement")]
    [SerializeField, Range(0.0f, 40f)] public float moveSpeed = 8f;
    [SerializeField, Range(0.0f, 10f)] public float turnSpeed = 7f;
    [SerializeField, Range(0.0f, 15f)] public float wallContext = 5f;
    [Header("Seperation")]
    [SerializeField, Range(0.0f, 30f)] public float seperationContext = 3f;
    [SerializeField, Range(0.0f, 10)] public float seperationStrength = 5f;
    [Header("Alignment")]
    [SerializeField, Range(0.0f, 30f)] public float alignmentContext = 7f;
    [SerializeField, Range(0.0f, 10)] public float alignmentStrength = 2.8f;
    [Header("Cohesion")]
    [SerializeField, Range(0.0f, 30f)] public float cohesionContext = 10f;
    [SerializeField, Range(0.0f, 10)] public float cohesionStrength = 2f;
    [Header("Fleeing")]
    [SerializeField, Range(0.0f, 5)] public float fleeingMoveSpeedMult = 3;
    [SerializeField, Range(0.0f, 30)] public float fleeContext = 5;
    [SerializeField, Range(0.0f, 2)] public float fleeTimeSeconds = 0.2f;
    [Header("Visuals")]
    [SerializeField] public Mesh model;
    [SerializeField] public Material material;
}
 