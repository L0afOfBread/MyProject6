using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using JetBrains.Annotations;

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
    private float attackCooldown = 2.0f; // Длительность анимации атаки
    private float lastAttackTime;

    public Slider playerHpSlider;
    void Start()
    {
        playerHpSlider.value = 100f;
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
            // Враг останавливается
            canWalk = false;
            agent.ResetPath(); // Останавливаем NavMeshAgent, чтобы он не толкал игрока
            enemyAnimator.SetBool("Walk", false);

            // Проверяем, прошло ли достаточно времени для следующего удара
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                enemyAnimator.SetTrigger("Attack");

                // Наносим урон
                PlayerController.playerHp -= 10;
                playerHpSlider.value = PlayerController.playerHp;

                lastAttackTime = Time.time;
            }
        }
    }
}
