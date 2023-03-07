using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FieldDrop : MonoBehaviour, IDropHandler
{
    CombatManager cm;
    PlayerInCombat player;

    public AudioClip lowEnergySFX;

    public Text lowEnergyText;
    void Start()
    {
        cm = CombatManager.instance;
        player = cm.player;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        { 

            //if (slotCategory == HeroSlotCategory.None)
            //    Debug.LogError("ERROR: Slot does not have a HeroSlotCategory selected!");
            if (CombatManager.instance.isPlayersTurn)
            {
                CardDisplay playedCard = eventData.pointerDrag.GetComponent<CardDisplay>();

                if (GameManager.instance.costActive == false || 
                    (GameManager.instance.costActive == true && player.energy >= playedCard.card.cost))
                {
                    player.energy -= playedCard.card.cost;
                    player.UpdateEnergy();
                    playedCard.ActivateCard();
                    CombatManager.instance.SendToDiscard(playedCard.card);
                    Destroy(eventData.pointerDrag.gameObject);
                    lowEnergyText.gameObject.SetActive(false);
                }
                else
                {
                    cm.PlayAudioClip(lowEnergySFX);
                    lowEnergyText.gameObject.SetActive(true);
                }
            }
        }
    }
}
