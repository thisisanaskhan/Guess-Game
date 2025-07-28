using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    
    public void OnClickPlayBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MatchCard");
    }

    
}
