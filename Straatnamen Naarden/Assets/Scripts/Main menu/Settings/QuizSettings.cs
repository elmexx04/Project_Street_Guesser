using UnityEngine;
using TMPro;

public class QuizSettings : MonoBehaviour
{
    public StreetQuizManager quizManager;

    public int poolSize = 10;
    public int guessDistance = 25;
    public int markAsLearned = 3;
    public int maxMinusScore = -2;
    public int pointsForCorrect = 1;
    public int pointsForWrong = -1;
    public int pointsForSkip = -1;

    public TextMeshProUGUI poolSizeText;
    public TextMeshProUGUI guessDistanceText;
    public TextMeshProUGUI markAsLearnedText;
    public TextMeshProUGUI maxMinusScoreText;
    public TextMeshProUGUI pointsForCorrectText;
    public TextMeshProUGUI pointsForWrongText;
    public TextMeshProUGUI pointsForSkipText;


    public void Start()
    {
        poolSizeText.text = poolSize.ToString();
        guessDistanceText.text = guessDistance.ToString() + " units";
        markAsLearnedText.text = markAsLearned.ToString();
        maxMinusScoreText.text = maxMinusScore.ToString();
        pointsForCorrectText.text = pointsForCorrect.ToString();
        pointsForWrongText.text = pointsForWrong.ToString();
        pointsForSkipText.text = pointsForSkip.ToString();
    }
    public void PlusPoolSize()
    {
        if (poolSize < 20)
        {
            poolSize++;
            quizManager.poolSize = poolSize;
            poolSizeText.text = poolSize.ToString();
        }
    }
    public void MinusPoolSize()
    {
        if (poolSize > 1)
        {
            poolSize--;
            quizManager.poolSize = poolSize;
            poolSizeText.text = poolSize.ToString();
        }
    }
    public void PlusGuessDistance()
    {
        if (guessDistance < 100)
        {
            guessDistance += 5;
            quizManager.guessDistance = guessDistance;
            guessDistanceText.text = guessDistance.ToString() + " units";
        }
    }
    public void MinusGuessDistance()
    {
        if (guessDistance > 5)
        {
            guessDistance -= 5;
            quizManager.guessDistance = guessDistance;
            guessDistanceText.text = guessDistance.ToString() + " units";
        }
    }
    public void PlusmarkAsLearned()
    {
        if (markAsLearned < 10)
        {
            markAsLearned++;
            quizManager.scoreForNext = markAsLearned;
            markAsLearnedText.text = markAsLearned.ToString();
        }
    }
    public void MinusmarkAsLearned()
    {
        if (markAsLearned > 1)
        {
            markAsLearned--;
            quizManager.scoreForNext = markAsLearned;
            markAsLearnedText.text = markAsLearned.ToString();
        }
    }
    public void PlusMaxMinusScore()
    {
        if (maxMinusScore > -10)
        {
            maxMinusScore--;
            quizManager.maxMinusScore = maxMinusScore;
            maxMinusScoreText.text = maxMinusScore.ToString();
        }
    }
    public void MinusMaxMinusScore()
    {
        if (maxMinusScore < -1)
        {
            maxMinusScore++;
            quizManager.maxMinusScore = maxMinusScore;
            maxMinusScoreText.text = maxMinusScore.ToString();
        }
    }
    public void PlusPointsForCorrect()
    {
        if (pointsForCorrect < 10)
        {
            pointsForCorrect++;
            quizManager.pointsForCorrect = pointsForCorrect;
            pointsForCorrectText.text = pointsForCorrect.ToString();
        }
    }
    public void MinusPointsForCorrect()
    {
        if (pointsForCorrect > 1)
        {
            pointsForCorrect--;
            quizManager.pointsForCorrect = pointsForCorrect;
            pointsForCorrectText.text = pointsForCorrect.ToString();
        }
    }
    public void PlusPointsForWrong()
    {
        if (pointsForWrong < 0)
        {
            pointsForWrong++;
            quizManager.pointsForWrong = pointsForWrong;
            pointsForWrongText.text = pointsForWrong.ToString();
        }
    }
    public void MinusPointsForWrong()
    {
        if (pointsForWrong > -10)
        {
            pointsForWrong--;
            quizManager.pointsForWrong = pointsForWrong;
            pointsForWrongText.text = pointsForWrong.ToString();
        }
    }
    public void PlusPointsForSkip()
    {
        if (pointsForSkip < 0)
        {
            pointsForSkip++;
            quizManager.pointsForSkip = pointsForSkip;
            pointsForSkipText.text = pointsForSkip.ToString();
        }
    }
    public void MinusPointsForSkip()
    {
        if (pointsForSkip > -10)
        {
            pointsForSkip--;
            quizManager.pointsForSkip = pointsForSkip;
            pointsForSkipText.text = pointsForSkip.ToString();
        }
    }
}
