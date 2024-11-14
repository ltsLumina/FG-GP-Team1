using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using DG.Tweening;

public class JellyMovement : MonoBehaviour
{

    [SerializeField] private float moveCoolDown;
    private float moveTimer = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveScale;
    private GameObject player;
    private Vector2 moveDir;
    private Vector2 playerDir;
    private Vector3 movePos;
    private Vector3 normalScale;
    private bool movingToTarget = true;
    [SerializeField] private float wallContext;
    [SerializeField] LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        moveTimer = moveCoolDown;
        normalScale = transform.localScale;
        FindTarget();
    }

    private void Update()
    {

        if (moveTimer < 0)
        {
            FindTarget();

        }

        if (movingToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime * moveSpeed * (Vector3.Distance(transform.position, movePos) + 0.1f));
        }
        else
        {
            transform.position += new Vector3(0, -2 * Time.deltaTime * Mathf.Min(Vector3.Distance(transform.position, movePos) + 0.1f,2), 0);
        }


        if (transform.position == movePos)
        {
            movingToTarget = false;
        }

        transform.localScale += new Vector3(0, 0.3f * -Time.deltaTime, 0);
        moveTimer -= Time.deltaTime;

    }

    private void FindTarget()
    {

        transform.localScale = normalScale;
        movingToTarget = true;
        moveTimer = moveCoolDown;

        float minMag = 10;
        int moveIndex = 0;
        List<Vector3> randDirs = new List<Vector3>();


        for (int i = 0; i <= 6; i++)
        {
            randDirs.Add(Random.insideUnitCircle);

            float currentMag = ((Vector2)randDirs[i] - Vector2.up).magnitude;
            if (currentMag < minMag)
            {
                minMag = currentMag;
                moveIndex = i;
            }
        }


        movePos = new Vector2(transform.position.x, transform.position.y) + (Vector2)randDirs[moveIndex] * moveScale;
    }


}


//avoid wall / sub
