using UnityEngine;

public class AnswerSystem : MonoBehaviour
{
    public float guessDistance = 25f;

    public bool IsCorrect(Vector2 guess, Vector2 correct)
    {
        float distance = Vector2.Distance(guess, correct);
        return distance <= guessDistance;
    }

    public float GetDistance(Vector2 guess, Vector2 correct)
    {
        return Vector2.Distance(guess, correct);
    }
}