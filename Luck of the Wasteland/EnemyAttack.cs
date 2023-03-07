using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "ScriptableObjects/Enemy Attacks")]
public class EnemyAttack : ScriptableObject
{
    public new string name;
    public int minAttack, maxAttack;
    public float time;

    public DamageType damageType;

    public AudioClip attackSFX;

    public GameObject attackAnim;

    public int GetDamage()
    {
        return Random.Range(minAttack, maxAttack + 1);
    }

}

