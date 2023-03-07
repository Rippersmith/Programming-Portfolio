using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObjects/Cards")]
public class Card : ScriptableObject
{
    public new string name;
    public int cost;
    public Sprite art;
    public int value = 0;

    public string description;

    public enum CardType { Normal, Tech, Mutation, Gear}
    public CardType cardType;

    public DamageType damageType;

    public AudioClip cardActivateSFX;

    public UnityEvent cardEvent;

    public GameObject animPrefab;

    //this is just for future versions of the game, don't work on this now!!!
    public Card upgradedCard;

    public void UpgradeCard()
    {
        name = upgradedCard.name;
        cost = upgradedCard.cost;
        art = upgradedCard.art;
        description = cardType.ToString() + "\n" + upgradedCard.description;
        cardType = upgradedCard.cardType;
    }

    #region Card Effects
    [SerializeField]
    public void DrawCards(int cards)
    {
        CombatManager.instance.DrawCards(cards);
        SpawnAnimOnPlayer();
    }

    public void AddBlock(int block)
    {
        CombatManager.instance.player.AddBlock(block);
        SpawnAnimOnPlayer();
    }

    public void AddAttackPower(int atkBoost)
    {
        float percentage = atkBoost / 100f;
        CombatManager.instance.DamageMod += percentage;
        SpawnAnimOnPlayer();
    }

    public void AddEnergy(int nrg)
    {
        CombatManager.instance.player.energy = nrg;
        SpawnAnimOnPlayer();
    }

    public void AttackRandomEnemy(int damage)
    {
        CombatManager.instance.AttackEnemy(damage, animPrefab, damageType);
    }

    public void AttackAllEnemies(int damage)
    {
        CombatManager.instance.AttackAllEnemies(damage, animPrefab, damageType);
    }

    public void HealPlayer(int healing)
    {
        CombatManager.instance.player.RecoverHealth(healing);
        //CombatManager.instance.playerHealth += healing;
        SpawnAnimOnPlayer();
    }

    void SpawnAnimOnPlayer()
    {
        if (animPrefab != null)
            Instantiate(animPrefab, CombatManager.instance.playerLoc.position, Quaternion.identity);
    }
    #endregion


}
