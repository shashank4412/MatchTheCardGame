using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
	[SerializeField] private StatsBoard statsBoard;
	[SerializeField] private AudioController audioController;
	[SerializeField] private Transform gameOverScreen;

	private ContentSizeFitter sizeFitter;
	private GridLayoutGroup layoutGroup;
	private Card cardPrefab;
	private Card lastOpenedCard;
	private Vault vault;
	private List<CardConfig> cardList = new List<CardConfig>();

	private const string CardList_KEY = "cardList";

	public void Start()
	{
		InitializeReferences();
		SpawnCards();
	}
	private void InitializeReferences() // Set initial references
	{
		lastOpenedCard = null;
		layoutGroup = GetComponent<GridLayoutGroup>();
		sizeFitter = GetComponent<ContentSizeFitter>();
		vault = new Vault();
		// Fetch card prefab from Resources/Prefab folder
		cardPrefab = Resources.Load<Card>("Prefab/CardPrefab");
	}

	#region Card Spawn events
	private void SpawnCards() // Spawn cards on the grid
	{
		// Initialize card data list
		try { cardList = vault.GetSavedData<List<CardConfig>>(CardList_KEY); }
		catch { }

		// Generate new cards if no saved data is present
		if (cardList == null || cardList.Count <= 0)
		{
			cardList = GenerateCards();
			vault.SaveData(cardList, CardList_KEY);
		}

		// Instantiate cards based on cardDataList
		foreach (CardConfig cardProp in cardList)
		{
			Card newCard = Instantiate(cardPrefab, transform);
			newCard.InitializeCard(cardProp, () => OnClickCard(newCard));
		}

		// Update card count on the stats board
		statsBoard.SetCardsCount(cardList.Count - statsBoard.matchesCount * 2);

		// Force layout build and disable auto-sizing
		Invoke("DisableAutoSizing", 0.1f);
	}
	private List<CardConfig> GenerateCards() // Generate card data for the game
	{
		// Create List of CardProperties from CardData
		List<CardConfig> cards = new List<CardConfig>();

		// Get text file with list of all card names
		TextAsset JSON = Resources.Load<TextAsset>("Cards/cardJSON");
		List<string> list = JsonUtility.FromJson<cardJSON>(JSON.text).cards;

		// shuffle the list and limit the number of pairs
		list = Shuffle(list).Take(statsBoard.pairLimit).ToList();

		// Make Pair/ double cards of each element in the list &
		// Shuffle the list again to randomize pairs
		list.AddRange(list);
		list = Shuffle(list);

		// For each card, Create CardConfig from cardfiles (present under Resources folder)
		foreach (string card in list)
			cards.Add(new CardConfig(card));

		return cards;
	}
	private List<string> Shuffle(List<string> list) // Shuffle the order of card data
	{
		if (list == null || list.Count <= 0) return new List<string>();

		// Shuffle the card data list
		for (int i = 0; i < list.Count; i++)
		{
			int randomIndex = Random.Range(i, list.Count);
			var temp = list[randomIndex];
			list[randomIndex] = list[i];
			list[i] = temp;
		}

		return list;
	}
	private void DisableAutoSizing() // Disable auto-sizing after layout build
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		sizeFitter.enabled = false;
		layoutGroup.enabled = false;
	}
	#endregion


	#region Card Behaviour events
	public void OnClickCard(Card card)
	{
		// Play flip audio
		audioController.Play(AudioType.CardFlipClip);

		if (lastOpenedCard == null) lastOpenedCard = card; // If no card is opened
		else HandleOpenedCard(card); // If a card is already opened
	}
	private void HandleOpenedCard(Card card)
	{
		// Increase the Moves count
		statsBoard.IncrementMovesCount();

		// If the clicked card matching with last opened card
		if (CardConfig.IsMatching(card.cardConfig, lastOpenedCard.cardConfig))
			HandleMatchedCard(card);
		else HandleMismatchedCard(card);

		// Reset last opened card
		lastOpenedCard = null;
	}
	private void HandleMatchedCard(Card card)
	{
		// Play success audio
		audioController.Play(AudioType.MatchSuccessClip);

		// Increment match count
		statsBoard.IncrementMatchesCount();

		// Remove and save data from cardDataList
		card.SetUsed();
		lastOpenedCard.SetUsed();
		vault.SaveData(cardList, CardList_KEY);

		// Update card count on the stats board
		statsBoard.IncrementCardsCount(-2);

		// Check if game over
		Invoke("CheckGameOver", 0.1f);
	}
	private void HandleMismatchedCard(Card card)
	{
		// Play fail audio
		audioController.Play(AudioType.MatchFailClip);

		// Flip back both cards
		card.HoldAndHide();
		lastOpenedCard.HoldAndHide();
	}
	private void CheckGameOver()
	{
		// Activate game over screen if every card isUsed
		gameOverScreen.gameObject.SetActive(statsBoard.cardsCount <= 0);
	}
	#endregion
}