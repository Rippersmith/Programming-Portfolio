using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHealthFunctions
{
    public int enemyHealth = 200;
    public bool isAttacking = false;

    [SerializeField] Text enemyHealthText;
    [SerializeField] EnemyAttack attack;
    [SerializeField] List<DamageType> weaknesses;

    public List<DamageType> WeaknessList() { return weaknesses; }

    public int rewardMoneyMin = 0, rewardMoneyMax = 0;
    public Card[] rewardCards;

    int enemyMaxHealth;
    float attackTimer = 0;
    float timeBuffer;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        enemyMaxHealth = enemyHealth;
        animator = GetComponent<Animator>();

        timeBuffer = attack.time / 5f;

        UpdateEnemyHealth();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (isAttacking == true && attackTimer <= 0)
        {
            animator.SetTrigger("Attack");
            CombatManager.instance.PlayAudioClip(attack.attackSFX);
            GetNewTime();
        }
    }

    //attack animation will play, which will in turn call this function to deal damage to the player
    public void ActivateAttackDamage()
    {
        CombatManager.instance.AttackPlayer(attack.GetDamage());
    }

    //lets add a lettle "extra time" so all enemies don't attack in unison
    public void ChangeEnemyAttackMode()
    {
        isAttacking = !isAttacking;
        if (isAttacking)
        {
            GetNewTime();
        }
    }

    void GetNewTime()
    {
        float rnd = Random.Range(-timeBuffer, timeBuffer);
        attackTimer = attack.time + rnd;
    }

    void UpdateEnemyHealth()
    {
        enemyHealthText.text = enemyHealth + "/" + enemyMaxHealth;
    }

    public void LoseHealth(int damage)
    {
        enemyHealth -= damage;
        UpdateEnemyHealth();

        if (enemyHealth <= 0)
        {
            CombatManager.instance.RemoveEnemyFromCombat(this);
        }
    }

    public void RecoverHealth(int healing)
    {
        enemyHealth += healing;
        UpdateEnemyHealth();
    }

    public int GetRewardMoney()
    {
        int rnd = Random.Range(rewardMoneyMin, rewardMoneyMax);
        return rnd;
    }

    public Card GetRewardCard()
    {
        int rnd = Random.Range(0, rewardCards.Length);
        return rewardCards[rnd];
    }
}
