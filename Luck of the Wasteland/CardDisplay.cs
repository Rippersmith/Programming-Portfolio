using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum DamageType { Normal, Fire, Acid, None};

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public Text cardName;
    public Text cardCost;
    public Text cardDesc;
    public Image cardArt;

    //Image cardBorder;

    private void Start()
    {
        if (card != null)
            DisplayFunc();
    }

    public CardDisplay(Card newCard)
    {
        card = newCard;

        DisplayFunc();
    }
    public void ReplaceCardData(Card newCard)
    {
        card = newCard;

        DisplayFunc();
    }


    void DisplayFunc()
    {
        cardName.text = card.name;
        cardCost.text = card.cost.ToString();

        string newDesc = "";
        if (card.cardType != Card.CardType.Normal)
            newDesc = card.cardType.ToString() + "\n";
        newDesc += card.description;
        cardDesc.text = newDesc;

        cardArt.sprite = card.art;

        //cardBorder = GetComponent<Image>();

        switch (card.cardType)
        {
            case Card.CardType.Tech:
                break;
        }
    }

    public void ActivateCard()
    {
        CombatManager.instance.PlayAudioClip(card.cardActivateSFX);
        card.cardEvent.Invoke();
    }

    public void SellCard()
    {
        GameManager gm = GameManager.instance;
        ShopScript ss = GameObject.Find("ShopCanvas").GetComponent<ShopScript>();

        if (gm.deck.Contains(card) && gm.money >= 20)
        {
            gm.money -= 20;
            gm.deck.Remove(card);
            ss.UpdateMoney();
            Destroy(gameObject);
        }
    }

    public void SellReward()
    {
        if (card.value > 0) GameManager.instance.money += card.value;
        else GameManager.instance.money += 5;

        CombatManager.instance.UpdateMoney();

        CombatManager.instance.rewardCards.Remove(card);
        Destroy(gameObject);
    }
}
