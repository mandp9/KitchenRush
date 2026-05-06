using UnityEngine;

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
    
}
