using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    static private int score;

    static ScoreManager()
    {
        score = -1;
    }

    static public void InitScore(string key, int maxVal)
    {
        score = maxVal;
        PlayerPrefs.DeleteKey(key);
    }

    static public int AddScore(int addScoreNum)
    {
        score += addScoreNum;
        return score;
    }

    static public int GetScore()
    {
        return score;
    }

    static public void SaveScore(string key)
    {
        PlayerPrefs.SetInt(key, score);
    }

    static public int ReadScore(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key);
        }
        else
        {
            return -1;
        }
    }

    static public void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
