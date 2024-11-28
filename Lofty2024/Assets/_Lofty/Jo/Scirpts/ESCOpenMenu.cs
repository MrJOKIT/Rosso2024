using UnityEngine;

public class ESCOpenMenu : MonoBehaviour
{
    public GameObject pauseMenu;  
    public GameObject SettingMenu;  
    private bool isPaused = false;


    void start()
    {
        pauseMenu.gameObject.SetActive(false);
            SettingMenu.gameObject.SetActive(false);
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                OpenMenu(); 
            }
        }
    }

   
    public void OpenMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;  
        isPaused = true;
    }

    
    public void ResumeGame()
    {
        pauseMenu.SetActive(false); 
        SettingMenu.SetActive(false); 

        Time.timeScale = 1f;  
        isPaused = false; 
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}