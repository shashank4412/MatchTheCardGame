using System;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardConfig cardConfig { get; private set; }
    private Image cardFace;
    private Button cardBtn;
    private Action onRevealAct;
    private bool isRevealed;

    private void OnEnable()
    {
        cardFace = GetComponent<Image>();
        cardBtn = GetComponent<Button>();

        cardBtn.onClick.AddListener(RevealCard);
    }

    public void InitializeCard(CardConfig cardConfig, Action onRevealAct)
    {
        this.cardConfig = cardConfig;
        this.onRevealAct = onRevealAct;

        if (cardConfig.isUsed)
        {
            SetUsed(); RevealCard();
        }
        else HideCard();
    }

    public void RevealCard()
    {
        if (isRevealed) return;
        isRevealed = true;
        cardBtn.enabled = false;
        cardFace.sprite = Resources.Load<Sprite>(cardConfig.cardFrontPath);
        if (!cardConfig.isUsed) this.onRevealAct.Invoke();
    }

    public void HoldAndHide()
    {
        Invoke("HideCard", 0.5f);
    }
    private void HideCard()
    {
        isRevealed = false;
        cardBtn.enabled = true;
        cardFace.sprite = Resources.Load<Sprite>(cardConfig.cardBackPath);
    }

    public void SetUsed()
    {
        cardConfig.isUsed = true;
        cardBtn.enabled = false;
        cardFace.color = new Color(1, 1, 1, 0.1f); // Make the card semi-transparent
    }
}