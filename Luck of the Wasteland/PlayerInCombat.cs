using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInCombat : MonoBehaviour, IHealthFunctions
{
    int playerHealth, maxHealth;
    int block = 0;

    public Text playerHealthText;
    [SerializeField] GameObject blockStat;
    Text playerBlockText;

    public int energy = 1;
    float timer = 1;
    public Image timerRadial;
    public Text energyText;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.costActive == false)
            energyText.gameObject.SetActive(false);

        playerHealth = GameManager.instance.playerHealth;
        maxHealth = GameManager.instance.playerMaxHealth;
        playerBlockText = blockStat.GetComponentInChildren<Text>();
        UpdateHealthUI();
        UpdateEnergy();
        UpdateBlock();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        timerRadial.fillAmount = 1 - timer;

        if (timer <= 0f && energy < 10)
        {
            energy++;
            timer = 1f;
            UpdateEnergy();
        }

    }

    public void UpdateEnergy()
    {
        energyText.text = energy.ToString();
    }

    public void AddBlock(int newBlock)
    {
        block += newBlock;
        UpdateBlock();
    }

    void UpdateBlock()
    {
        if (block <= 0)
        {
            blockStat.SetActive(false);
        }
        else
        {
            blockStat.SetActive(true);
        }

        playerBlockText.text = block.ToString();
    }

    public void LoseHealth(int damage)
    {
        if (block > 0)
        {
            if (block - damage < 0)
            {
                damage -= block;
                block = 0;
                UpdateBlock();
            }
            else
            {
                block -= damage;
                UpdateBlock();
                return;
            }
        }

        playerHealth = Mathf.Max(playerHealth - damage, 0);
        UpdateHealthUI();

        if (playerHealth <= 0)
        {
            //Debug.LogError("GAME OVER");

            CombatManager.instance.LoseCombat();
        }
    }

    public void RecoverHealth(int healing)
    {
        playerHealth = Mathf.Min(playerHealth + healing, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        playerHealthText.text = playerHealth + "/" + maxHealth;
    }

}
