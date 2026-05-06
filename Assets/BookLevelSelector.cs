using UnityEngine;
using UnityEngine.SceneManagement;

public class BookLevelSelector : MonoBehaviour
{
    [Header("Página Izquierda")]
    public GameObject spread0_Left;   // Btn_New
    public GameObject spread1_Left;   // Btn_Level2

    [Header("Página Derecha")]
    public GameObject spread0_Right;  // Btn_Level1 + Btn_Forward
    public GameObject spread1_Right;  // Btn_Back + Btn_Level3

    private int currentSpread = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowSpread(0);
    }

    public void NextPage()
    {
        if (currentSpread >= 1) return;
        currentSpread++;
        ShowSpread(currentSpread);
    }

    public void PreviousPage()
    {
        if (currentSpread <= 0) return;
        currentSpread--;
        ShowSpread(currentSpread);
    }

    void ShowSpread(int index)
    {
        spread0_Left.SetActive(index == 0);
        spread0_Right.SetActive(index == 0);
        spread1_Left.SetActive(index == 1);
        spread1_Right.SetActive(index == 1);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
