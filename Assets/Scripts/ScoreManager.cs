using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score;
    public static int coinsCollected;

    void Start()
    {
        // resets the score and coins collected to 0 at the start of the game
        score = 0;
        coinsCollected = 0;
    }

    public static void CollectCoin()
    {
        coinsCollected++;
        score += 10;

        if (coinsCollected >= 10)
        {
            coinsCollected = 0;
            PlayerMovement.Instance.RestoreHealth(10); // Add 10 to health since each health point represents 10 in the slider
        }
    }
}
