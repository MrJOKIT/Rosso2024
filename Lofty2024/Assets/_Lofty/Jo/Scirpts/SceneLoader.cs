using UnityEngine;
using UnityEngine.SceneManagement;  

public class SceneLoader : MonoBehaviour
{
    
    public void LoadScene(string sceneName)
    {
        FormatAllData();
        SceneManager.LoadScene(sceneName);
    }
    
    private void FormatAllData()
    {
        ES3.DeleteKey("FirstStage");
        ES3.DeleteKey("SecondStage");
        ES3.DeleteKey("ClearCount");
        
        ES3.DeleteKey("CardStock");
        
        ES3.DeleteKey("EricCoin");
        ES3.DeleteKey("FlameSoul");
        
        ES3.DeleteKey("PlayerDefaultHealth");
        ES3.DeleteKey("PlayerDefaultHealthTemp");
        ES3.DeleteKey("PlayerCurrentHealth");
        ES3.DeleteKey("PlayerCurrentHealthTemp");
        
        ES3.DeleteKey("PlayerDefaultMovePoint");
        ES3.DeleteKey("PlayerDefaultDamage");
        ES3.DeleteKey("PlayerDefaultKnockBackRange");
        
        ES3.DeleteKey("ArtifactHave");
        ES3.DeleteKey("ClassType");
        ES3.DeleteKey("EyeKing");
        ES3.DeleteKey("LastChance");
        ES3.DeleteKey("Tutorial");
        ES3.DeleteKey("TutorialPopUp");
        ES3.DeleteKey("PlayerAbility");
        
        ES3.DeleteKey("FirstClassUnlock");
        ES3.DeleteKey("SecondClassUnlock");
        ES3.DeleteKey("SwordPassiveOne");
        ES3.DeleteKey("SwordPassiveTwo");
        ES3.DeleteKey("BladePassiveOne");
        ES3.DeleteKey("BladePassiveTwo");
        ES3.DeleteKey("ShootPassiveOne");
        ES3.DeleteKey("ShootPassiveTwo");
    }
}