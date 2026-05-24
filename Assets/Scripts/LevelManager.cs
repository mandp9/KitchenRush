using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Level Settings")]
    public int totalLevelCustomers = 10;
    public float delayBeforeEnd = 5.0f;
    private int customersServed = 0;

    [Header("Audio Settings")]
    public AudioSource levelCompletedAudio;

    void Awake()
    {
        instance = this;
    }

    public void RegisterCustomerFinished()
    {
        customersServed++;

        if (customersServed >= totalLevelCustomers)
        {
            StartCoroutine(WaitAndEndLevel());
        }
    }

    IEnumerator WaitAndEndLevel()
    {
        if (levelCompletedAudio != null)
        {
            levelCompletedAudio.Play();
        }

        yield return new WaitForSeconds(delayBeforeEnd);
        SceneManager.LoadScene("StartMenuFinal"); 
    }
}