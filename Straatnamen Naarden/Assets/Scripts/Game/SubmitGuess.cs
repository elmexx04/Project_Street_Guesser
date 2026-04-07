using UnityEngine;

public class SubmitGuess : MonoBehaviour
{
    public AnswerSystem answerSystem;
    public ProgressSystem progressSystem;

    public void SubmitGuess2()
    {
        if (!mapClick.hasClicked) return;

        Vector2 guess = mapClick.lastClickPosition;
        Vector2 correct = currentStreet.GetPositionForRegion(selectMode.currentRegion);

        bool isCorrect = answerSystem.IsCorrect(guess, correct);

        if (isCorrect)
        {
            progressSystem.AddScore(
                currentStreet.streetName,
                selectMode.currentRegion,
                pointsForCorrect,
                maxMinusScore,
                scoreForNext
            );

            feedbackText.text = "Correct!";
        }
        else
        {
            float distance = answerSystem.GetDistance(guess, correct);

            progressSystem.AddScore(
                currentStreet.streetName,
                selectMode.currentRegion,
                pointsForWrong,
                maxMinusScore,
                scoreForNext
            );

            feedbackText.text = "Fout! Afstand: " + Mathf.RoundToInt(distance);
        }
}