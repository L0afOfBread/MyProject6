using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Vector3 randomDirection;
    private float changeDirectionTimer;
    private float minChange = 3f;
    private float maxChange = 8f;
    public Animator enemyAnimator;
    private bool canWalk = true;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChangeDirection();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorClipInfo[] clipInfo = enemyAnimator.GetCurrentAnimatorClipInfo(0);
        Debug.Log(clipInfo[0].clip.name);
        if (Vector3.Distance(transform.position, player.position) <= 25f && canWalk == true)
        {
            agent.SetDestination(player.position);
            enemyAnimator.SetBool("Walk", true);
        }
        else if (canWalk == true)
        {
            enemyAnimator.SetBool("Walk", true);
            changeDirectionTimer -= Time.deltaTime;
            if (changeDirectionTimer <= 0f)
            {
                ChangeDirection();
            }
            agent.SetDestination(transform.position + randomDirection);
        }
        if (clipInfo[0].clip.name != "EnemyAttack")
        {
            canWalk = true;
        }
    }

    void ChangeDirection()
    {
        randomDirection = Random.insideUnitSphere * 10f;
        changeDirectionTimer = Random.Range(minChange, maxChange);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canWalk = false;
            enemyAnimator.SetBool("Walk", false);
            enemyAnimator.SetTrigger("Attack");
        }
    }
}
