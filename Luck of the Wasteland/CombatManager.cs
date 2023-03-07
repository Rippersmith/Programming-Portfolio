using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    //Camera mainCamera;
    AudioSource audioPlayer;

    public GameObject cardPrefab;
    public GameObject playersHand;

    public int playerHealth = 100;

    public PlayerInCombat player;

    [SerializeField] List<Card> drawPile;
    [SerializeField] List<Card> cardsInHand;
    [SerializeField] List<Card> discardPile;

    [SerializeField] Text drawPileText;
    [SerializeField] Text discardPileText;

    public bool isPlayersTurn = true;

    public GameObject winScreen;
    public GameObject loseScreen;
    public AudioClip winMusic, loseMusic;

    [SerializeField] Text moneyText;
    [SerializeField] Text currMoneyText;

    public Transform playerLoc;
    [SerializeField] protected List<Enemy> enemyParty;
    public Transform[] enemySpawnLocs;

    public int rewardMoney = 0;

    public GameObject cardRewardPrefab;
    public List<Card> rewardCards;

    //public 

    //for mods: 1 = 100%, will multiply damage using this variable
    [SerializeField] float damageMod = 1;
    public float DamageMod
    {
        get { return damageMod; }
        set { damageMod = value; }
    }

    //don't add this unless you start adding cards that remove cards from play
    //List<Card> removedCards;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        instance = this;

        //mainCamera = Camera.main;
        audioPlayer = GetComponent<AudioSource>();

        SummonNewEnemies();

        drawPile.AddRange(GameManager.instance.deck);
        Shuffle();

        DrawCards(7);
    }

    private void UpdateDrawAndDiscard()
    {
        drawPileText.text = drawPile.Count.ToString();
        discardPileText.text = discardPile.Count.ToString();
    }

    protected virtual void SummonNewEnemies()
    {
        int rnd1 = Random.Range(2, 5);

        for (int i = 0; i < rnd1; i++)
        {
            Enemy newEnemy = Instantiate(GameManager.instance.GetNewEnemy(), enemySpawnLocs[i].position, Quaternion.identity);

            enemyParty.Add(newEnemy);

            rewardMoney += newEnemy.GetRewardMoney();
            rewardCards.Add(newEnemy.GetRewardCard());
        }
    }

    //TODO: move this function to another script, maybe an AudioManager
    public void PlayAudioClip(AudioClip newClip)
    {
        audioPlayer.PlayOneShot(newClip);
    }


    #region Hand/Deck/Discard Functions

    public void DrawCardForEnergy()
    {
        if ((drawPile.Count + discardPile.Count >= 1) && isPlayersTurn == true &&
            (GameManager.instance.costActive == false || (GameManager.instance.costActive == true && player.energy >= 1)))
        {
            DrawCards(1);
            player.energy--;
            player.UpdateEnergy();
        }
        return;
    }

    public void DrawCards(int drawCards)
    {
        //if ALL cards are currently in your hand, do NOTHING
        if (drawPile.Count < 1 && discardPile.Count < 1) return;

        if (discardPile.Count + drawPile.Count <= drawCards) drawCards = discardPile.Count + drawPile.Count;

        for (int i = 1; i <= drawCards; i++)
        {
            //This code is for testing -- uncomment it if you run into an error when trying to draw from the deck!
            //Debug.LogWarning("Drawing " + i + " of " + drawCards + " /// Deck: " + drawPile.Count + " -- Discard: " + discardPile.Count);
            if (drawPile.Count < 1)
                DeckIsEmptied();

            Card c = ScriptableObject.CreateInstance<Card>();

            GameObject newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(playersHand.transform);
            newCard.GetComponent<CardDisplay>().ReplaceCardData(drawPile[0]);
            newCard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            cardsInHand.Add(drawPile[0]);
            drawPile.RemoveAt(0);

            UpdateDrawAndDiscard();
        }
    }

    public void SendToDiscard(Card discardedCard)
    {
        if (cardsInHand.Contains(discardedCard))
            cardsInHand.Remove(discardedCard);

        discardPile.Add(discardedCard);
        UpdateDrawAndDiscard();
    }

    void DeckIsEmptied()
    {
        //if ALL cards are currently in your hand, do NOTHING
        if (drawPile.Count < 1 && discardPile.Count < 1) return;

        RefillDeck();
        Shuffle();
    }

    public void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        UpdateDrawAndDiscard();
    }

    public void Shuffle()
    {
        System.Random rnd = new System.Random();

        for (int i = 0; i < drawPile.Count; i++)
        {
            int tmp = rnd.Next(i, drawPile.Count);
            Card temporary = drawPile[i];
            drawPile[i] = drawPile[tmp];
            drawPile[tmp] = temporary;
        }
    }

    #endregion

    #region Combat Functions

    public bool IsPlayersTurn()
    {
        return isPlayersTurn;
    }

    public void ChangeTurns()
    {
        isPlayersTurn = !isPlayersTurn;

        if (isPlayersTurn == true)
        {
            ResetModifiers();
            DrawCards(5);
        }

        foreach (Enemy e in enemyParty)
        {
            e.ChangeEnemyAttackMode();
        }
    }

    public void AttackEnemy(int damage, GameObject anim, DamageType attackType = DamageType.Normal)
    {
        int enemyNum = Random.Range(0, enemyParty.Count);

        damage = Mathf.FloorToInt(damage * damageMod);

        Enemy target = enemyParty[enemyNum];

        if (target.WeaknessList().Count > 0 &&
            target.WeaknessList().Contains(attackType))
        {
            damage *= 2;
        }

        if (anim != null)
        {
            Instantiate(anim, target.transform.position, Quaternion.identity);
        }

        target.LoseHealth(damage);
    }

    public void AttackAllEnemies(int damage, GameObject anim, DamageType attackType = DamageType.Normal)
    {
        damage = Mathf.FloorToInt(damage * damageMod);
        int newDam = 0;

        for (int e = 0; e < enemyParty.Count; e++)
        {
            Enemy target = enemyParty[e];
            newDam = damage;

            Instantiate(anim, target.transform.position, Quaternion.identity);

            if (target.WeaknessList().Count > 0 &&
                target.WeaknessList().Contains(attackType))
                newDam *= 2;

            if (target.enemyHealth - newDam <= 0) e--;

            target.LoseHealth(newDam);
        }
    }

    #endregion

    #region Damage Modifications
    public void ResetModifiers()
    {
        damageMod = 1;
    }

    #endregion

    public void AttackPlayer(int damage)
    {
        player.LoseHealth(damage);
    }

    public void RemoveEnemyFromCombat(Enemy enemy)
    {
        enemyParty.Remove(enemy);
        Destroy(enemy.gameObject);

        if (enemyParty.Count < 1)
        {
            WinCombat();
        }
    }

    //this is for TESTING ONLY, it will auto-win the encounter & go straight to the rewards screen
    public void AutoWin()
    {
        //foreach(Enemy e in enemyParty)
        while (enemyParty.Count > 0)
        {
            enemyParty[0].LoseHealth(10000);
        }
    }

    public void AutoLose()
    {
        player.LoseHealth(10000);
    }


    //TODO: get list from card rewards, display cards, and present them as buttons to the player
    void WinCombat()
    {
        AudioManager.instance.StopMusic();
        audioPlayer.PlayOneShot(winMusic);

        winScreen.SetActive(true);

        moneyText.text = "  +   $" + rewardMoney;
        GameManager.instance.money += rewardMoney;
        UpdateMoney();

        for (int c = 0; c < rewardCards.Count; c++)
        {
            GameObject newRewardCard = Instantiate(cardRewardPrefab, Vector3.zero, Quaternion.identity);
            newRewardCard.GetComponent<CardDisplay>().ReplaceCardData(rewardCards[c]);
            newRewardCard.transform.SetParent(winScreen.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform);
            newRewardCard.transform.localScale = Vector3.one;
            //GameManager.instance.deck.Add(rewardCards[c]);
        }
    }

    public void UpdateMoney()
    {
        if (currMoneyText != null)
            currMoneyText.text = "Money: " + GameManager.instance.money.ToString();
    }

    public void ReturnToMap()
    {
        foreach (Card c in rewardCards)
        {
            GameManager.instance.deck.Add(c);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("CustomMapScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void LoseCombat()
    {
        AudioManager.instance.StopMusic();
        audioPlayer.PlayOneShot(loseMusic);
        loseScreen.SetActive(true);
    }

}
