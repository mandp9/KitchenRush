using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreController : MonoBehaviour
{
    // singleton setup
    public static ScoreController instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else
        {
            instance = this;
            // DontDestroyOnLoad(this);
        }
    }

    public int score = 0;

    [Header("Text fields")]
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text gainText;

    private Color32 white = new Color32(255, 255, 255, 255);
    private Color32 green = new Color32(63, 255, 63, 255);
    private Color32 red = new Color32(255, 63, 63, 255);

    private string scorePrefix = "";
    private string gainPrefix = "";

    private void Start()
    {
        gainText.text = "";
    }

    private void Update()
    {
        timeText.text = System.DateTime.Now.ToString("HH:mm");
    }

    public void UpdateScore(int gain)
    {
        score += gain;

        if (score > 0) scorePrefix = "+";
        else scorePrefix = "";
        scoreText.text = scorePrefix + score.ToString() + " rep";

        StartCoroutine(FlashGain(gain));
    }

    // @unity would it kill you to let me sleep()
    private IEnumerator FlashGain(int gain)
    {
        if (gain > 0)
            gainText.faceColor = green;
        if (gain < 0)
            gainText.faceColor = red;

        if (gain > 0) gainPrefix = "+";
        else gainPrefix = "";

        for (int i = 0; i < 5; i++)
        {
            gainText.text = "\n\n" + gainPrefix + gain.ToString() + " rep";
            yield return new WaitForSeconds(0.5f);
            gainText.text = "";
            yield return new WaitForSeconds(0.5f);
        }

        gainText.faceColor = white;
    }

}
