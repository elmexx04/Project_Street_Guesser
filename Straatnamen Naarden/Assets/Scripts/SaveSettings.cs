using UnityEngine;

public class SaveSettings : MonoBehaviour
{
	public QuizSettings quizSettings;

	const string KeyPoolSize = "QS_PoolSize";
	const string KeyGuessDistance = "QS_GuessDistance";
	const string KeyMarkAsLearned = "QS_MarkAsLearned";
	const string KeyMaxMinusScore = "QS_MaxMinusScore";
	const string KeyPointsForCorrect = "QS_PointsForCorrect";
	const string KeyPointsForWrong = "QS_PointsForWrong";
	const string KeyPointsForSkip = "QS_PointsForSkip";

	void Start()
	{
		LoadSettings();
	}

	public void SaveQuizSettings()
	{
		if (quizSettings == null) return;

		PlayerPrefs.SetInt(KeyPoolSize, quizSettings.poolSize);
		PlayerPrefs.SetInt(KeyGuessDistance, quizSettings.guessDistance);
		PlayerPrefs.SetInt(KeyMarkAsLearned, quizSettings.markAsLearned);
		PlayerPrefs.SetInt(KeyMaxMinusScore, quizSettings.maxMinusScore);
		PlayerPrefs.SetInt(KeyPointsForCorrect, quizSettings.pointsForCorrect);
		PlayerPrefs.SetInt(KeyPointsForWrong, quizSettings.pointsForWrong);
		PlayerPrefs.SetInt(KeyPointsForSkip, quizSettings.pointsForSkip);

		PlayerPrefs.Save();
	}

	public void LoadSettings()
	{
		if (quizSettings == null) return;

		// Only overwrite if a value exists in PlayerPrefs, otherwise keep defaults
		if (PlayerPrefs.HasKey(KeyPoolSize)) quizSettings.poolSize = PlayerPrefs.GetInt(KeyPoolSize);
		if (PlayerPrefs.HasKey(KeyGuessDistance)) quizSettings.guessDistance = PlayerPrefs.GetInt(KeyGuessDistance);
		if (PlayerPrefs.HasKey(KeyMarkAsLearned)) quizSettings.markAsLearned = PlayerPrefs.GetInt(KeyMarkAsLearned);
		if (PlayerPrefs.HasKey(KeyMaxMinusScore)) quizSettings.maxMinusScore = PlayerPrefs.GetInt(KeyMaxMinusScore);
		if (PlayerPrefs.HasKey(KeyPointsForCorrect)) quizSettings.pointsForCorrect = PlayerPrefs.GetInt(KeyPointsForCorrect);
		if (PlayerPrefs.HasKey(KeyPointsForWrong)) quizSettings.pointsForWrong = PlayerPrefs.GetInt(KeyPointsForWrong);
		if (PlayerPrefs.HasKey(KeyPointsForSkip)) quizSettings.pointsForSkip = PlayerPrefs.GetInt(KeyPointsForSkip);

		quizSettings.UpdateUI();
        quizSettings.LinkQuizManager();
	}

	public void ResetToDefaults()
	{
		if (quizSettings == null) return;

		quizSettings.poolSize = 10;
		quizSettings.guessDistance = 25;
		quizSettings.markAsLearned = 3;
		quizSettings.maxMinusScore = -2;
		quizSettings.pointsForCorrect = 1;
		quizSettings.pointsForWrong = -1;
		quizSettings.pointsForSkip = -1;

		// Remove saved keys so defaults will be used on next load
		PlayerPrefs.DeleteKey(KeyPoolSize);
		PlayerPrefs.DeleteKey(KeyGuessDistance);
		PlayerPrefs.DeleteKey(KeyMarkAsLearned);
		PlayerPrefs.DeleteKey(KeyMaxMinusScore);
		PlayerPrefs.DeleteKey(KeyPointsForCorrect);
		PlayerPrefs.DeleteKey(KeyPointsForWrong);
		PlayerPrefs.DeleteKey(KeyPointsForSkip);

		PlayerPrefs.Save();

		quizSettings.UpdateUI();
        quizSettings.LinkQuizManager();
	}
}
