using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Text moneyText;
    public Text moneyText2;

    public Transform stockHolder;
    public GameObject sellableCardPrefab;
    public List<Card> availableStock;

    public AudioSource audioSource;
    public AudioClip moneySound;

    public Text shopName;
    string[] randAdjectives = { "The  Amazing", "Jeffo's", "Remarkable", "Lost  and  Found", "Extra", "Karimi's", "Grax's", "Leftover", "Your  New", "Tactical", "The  Random", "Apocalyptic" };
    string[] randNouns = { "Junk", "Treasures", "Artifacts", "Trash", "Enhancements", "Upgrades", "Cards", "Jam", "Shop", "Merchant", "Traders" };

    public GameObject removeFromDeckWindow;
    public GameObject removableCardPrefab;
    [SerializeField] Transform removableCardHolder;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        int rand1 = Random.Range(0, randAdjectives.Length);
        int rand2 = Random.Range(0, randNouns.Length);

        shopName.text = randAdjectives[rand1] + "  " + randNouns[rand2];

        UpdateMoney();

        for (int i = 0; i < 10; i++)
        {
            int c = Random.Range(0, availableStock.Count);
            SpawnNewCard(availableStock[c]);
        }
    }

    public void SpawnNewCard(Card newCard)
    {
        GameObject newItem = Instantiate(sellableCardPrefab, Vector3.zero, Quaternion.identity);
        newItem.transform.SetParent(stockHolder);
        newItem.transform.localScale = Vector3.one * 1.2f;

        CardDisplay itemCD = newItem.GetComponentInChildren<CardDisplay>();
        itemCD.ReplaceCardData(newCard);
    }

    public void OpenCloseRemovableCards()
    {
        removeFromDeckWindow.SetActive(!removeFromDeckWindow.activeSelf);
        if (removeFromDeckWindow.activeSelf == true)
        {
            UpdateDeck();
        }
    }

    void UpdateDeck()
    {
        foreach (Transform child in removableCardHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        FillOutDeckLibrary();
    }

    void FillOutDeckLibrary()
    {
        List<Card> cardLibrary = GameManager.instance.deck;

        cardLibrary.Sort(SortByName);

        for (int c = 0; c < cardLibrary.Count; c++)
        {
            DisplayNewCardInLibrary(cardLibrary[c]);
        }
    }

    static int SortByName(Card c1, Card c2)
    {
        return c1.name.CompareTo(c2.name);
    }
    public void DisplayNewCardInLibrary(Card card)
    {
        CardDisplay newCardDisplay = Instantiate(removableCardPrefab, Vector3.zero, Quaternion.identity).GetComponent<CardDisplay>();

        newCardDisplay.gameObject.transform.SetParent(removableCardHolder);
        newCardDisplay.gameObject.transform.localScale = Vector3.one * 2f;

        newCardDisplay.ReplaceCardData(card);
    }


    public void UpdateMoney()
    {
        audioSource.PlayOneShot(moneySound);
        moneyText.text = "Money: " + GameManager.instance.money;
        moneyText2.text = "Money: " + GameManager.instance.money;
    }

}
