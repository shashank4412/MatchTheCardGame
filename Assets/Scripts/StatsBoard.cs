using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsBoard : MonoBehaviour
{
	private Vault vault;
	public Button resetBtn;
	public Text matchesTxt;
	public Text movesTxt;
	public Text cardsTxt;
	public Text pairLimitTxt;
	public Slider pairLimitSlider;

	public int movesCount { get; private set; }
	public int matchesCount { get; private set; }
	public int cardsCount { get; private set; }
	public int pairLimit { get; private set; }

	private const string MatchesCount_KEY = "matchesCount";
	private const string MovesCount_KEY = "movesCount";
	private const string PairLimit_KEY = "pairLimit";

	private void Start()
	{
		vault = new Vault();

		SetMatchesCount(vault.GetSavedData<int>(MatchesCount_KEY));
		SetMovesCount(vault.GetSavedData<int>(MovesCount_KEY));
		SetPairLimit(vault.GetSavedData<int>(PairLimit_KEY, 14));

		resetBtn.onClick.AddListener(Reset);
		pairLimitSlider.onValueChanged.AddListener(ChangePairLimit);
	}


	public void IncrementMatchesCount(int value = 1)
	{
		SetMatchesCount(matchesCount + value);
	}
	public void IncrementMovesCount(int value = 1)
	{
		SetMovesCount(movesCount + value);
	}
	public void IncrementCardsCount(int value)
	{
		SetCardsCount(cardsCount + value);
	}
	private void ChangePairLimit(float newLimit)
	{
		int pairLimit = Mathf.RoundToInt(newLimit);
		vault.DeleteAllData();
		SetPairLimit(pairLimit);
		Reload();
	}


	public void SetMatchesCount(int matches)
	{
		this.matchesCount = matches;
		matchesTxt.text = "Matches: " + matches;
		vault.SaveData(matches, MatchesCount_KEY);
	}
	public void SetMovesCount(int moves)
	{
		this.movesCount = moves;
		movesTxt.text = "Moves: " + moves;
		vault.SaveData(moves, MovesCount_KEY);
	}
	public void SetCardsCount(int cardsCount)
	{
		this.cardsCount = cardsCount;
		cardsTxt.text = "Cards: " + cardsCount;
	}
	public void SetPairLimit(int pairLimit)
	{
		this.pairLimit = pairLimit;
		pairLimitTxt.text = "CardPairs <b>" + pairLimit + "</b>";
		pairLimitSlider.value = pairLimit;
		vault.SaveData(pairLimit, PairLimit_KEY);
	}

	private void Reset()
	{
		vault.DeleteAllData();
		Reload();
	}
	private void Reload()
	{
		SceneManager.LoadScene(0);
	}
}