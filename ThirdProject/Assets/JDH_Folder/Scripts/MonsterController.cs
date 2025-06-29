using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Movement")]
    public float stopDistance = 1.2f;

    [Header("Health")]
    public int maxHp = 100;
    public int currentHp = 0;

    [Header("Attack")]
    public int damage = 10;
    public float attackStartDistance = 2.0f;
    public float attackDistance = 1.2f;
    public float attackCooldown = 2f;

    [Header("Audio Clips")]
    public AudioClip walkClip;
    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private Transform target;
    private Animator animator;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private float lastAttackTime = 0f;

    private bool isAttacking = false;
    private bool isHit = false;
    private bool isDead = false;

    void Start()
    {
        currentHp = maxHp;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (target == null || isDead || isHit) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (!isAttacking && distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator?.SetBool("isWalking", true);
            PlayLoopingSound(walkClip);
        }
        else if (distance <= attackStartDistance)
        {
            Attack();
        }
        else
        {
            StopMoving();
        }
    }

    void StopMoving()
    {
        agent.isStopped = true;
        animator?.SetBool("isWalking", false);
        StopSound();
    }

    void Attack()
    {
        StopMoving();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator?.SetTrigger("Attack");
            isAttacking = true;
            lastAttackTime = Time.time;

            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void DealDamage()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackDistance)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        animator?.SetTrigger("Hit");
        isHit = true;

        StopSound(); // 걷는 소리 중단
        PlayOneShotSound(hitClip); // 피격 소리

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void EndHit()
    {
        isHit = false;
    }

    void Die()
    {
        isDead = true;
        StopMoving();
        animator?.SetTrigger("Die");
        PlayOneShotSound(deathClip);
        Destroy(gameObject, 2f);
    }

    public int GetCurrentHealth()
    {
        return currentHp;
    }

    // 애니메이션 이벤트로 호출
    public void PlayAttackSound()
    {
        StopSound();
        PlayOneShotSound(attackClip);
    }

    void PlayLoopingSound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void PlayOneShotSound(AudioClip clip)
    {
        if (clip == null || audioSource == null);
        audioSource.PlayOneShot(clip);
    }

    void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }
    }
}
