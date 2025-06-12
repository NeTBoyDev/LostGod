using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public void LoadGameplay()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
