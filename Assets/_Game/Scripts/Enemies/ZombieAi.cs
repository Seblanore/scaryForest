using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ZombieAi : NetworkBehaviour
{
    public LayerMask whatIsGround, whatIsPlayer;

    private Transform player;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float damage;

    //States
    public float sightRange, meleeAttackRange;
    public bool playerInSightRange, playerInMeleeAttackRange;

    private Animator Animator;
    private NavMeshAgent Agent;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;

    private String State;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Agent.updatePosition = false;
        Agent.updateRotation = true;
    }

    private void OnAnimatorMove()
    {
        if (!IsOwner) return;

        Vector3 rootPosition = Animator.rootPosition;
        rootPosition.y = Agent.nextPosition.y;
        transform.position = rootPosition;
        Agent.nextPosition = rootPosition;
    }

    public void Update()
    {
        if (!IsOwner) return;

        player = FindTarget();

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInMeleeAttackRange = Physics.CheckSphere(transform.position, meleeAttackRange, whatIsPlayer);

        // FIXME
        if (!playerInSightRange && !playerInMeleeAttackRange) Patroling();
        if (playerInSightRange && !playerInMeleeAttackRange) ChasePlayer();
        if (playerInSightRange && playerInMeleeAttackRange) AttackPlayer();

        //SynchronizeAnimatorAndAgent();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            Agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        Animator.SetBool("move", true);
        Animator.SetBool("attack", false);
        Animator.SetBool("chase", false);
    }

    private void SearchWalkPoint()
    {
        //Calculte random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y+0.2f, transform.position.z + randomZ);

        Ray ray = new Ray(walkPoint, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit rayHit, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
        
    }

    private void ChasePlayer()
    {
        Agent.SetDestination(player.position);
        Animator.SetBool("chase", true);
        Animator.SetBool("move", false);
        Animator.SetBool("attack", false);
        
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        Agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            Debug.Log("Zombie attacked player " + player);
            //Attack code here
            player.GetComponent<PlayerHealth>().TakeDamage(damage);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        Animator.SetBool("attack", true);
        Animator.SetBool("move", false);
        Animator.SetBool("chase", false);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Return the closest player
    public Transform FindTarget()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        Transform closest;

        if (candidates.Length == 0)
            return null;

        closest = candidates[0].transform;
        for (int i = 0; i < candidates.Length; ++i)
        {
            float distance = (candidates[i].transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                closest = candidates[i].transform;
                minDistance = distance;
            }
        }
        return closest;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = Agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        Velocity = SmoothDeltaPosition / Time.deltaTime;
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            Velocity = Vector2.Lerp(
                Vector2.zero,
                Velocity,
                Agent.remainingDistance / Agent.stoppingDistance
            );
        }

        bool shouldMove = Velocity.magnitude > 0.5f
            && Agent.remainingDistance > Agent.stoppingDistance;

        Animator.SetBool("chase", shouldMove);

        Animator.SetFloat("locomotion", Velocity.magnitude);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > Agent.radius / 2f)
        {
            transform.position = Vector3.Lerp(
                Animator.rootPosition,
                Agent.nextPosition,
                smooth
            );
        }
    }

}
