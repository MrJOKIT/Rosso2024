using UnityEngine;
using UnityEngine.UI;  

public class ToggleSoundOnActivation : MonoBehaviour
{
    private SoundManage soundManager;
    private bool isActive = false;
    
    void Start()
    {
        soundManager = SoundManage.Instance;
        
    }

    void Update()
    {
        if (gameObject.activeInHierarchy && !isActive)
        {
            PlayEndCredits();
            isActive = true;
        }
        else if (!gameObject.activeInHierarchy && isActive)
        {
            PlayMusic();
            isActive = false;
        }
    }

    private void PlayEndCredits()
    {
        soundManager.BGaudioSource.Stop();
        soundManager.PlayBackgroundMusic(Sound.EndCredits); 
    }

    private void PlayMusic()
    {
        soundManager.PlayBackgroundMusic(Sound.Music1); 
    }

    
    public void ChangeToMusic1()
    {
        soundManager.BGaudioSource.Stop(); 
        soundManager.PlayBackgroundMusic(Sound.Music1); 
    }
}