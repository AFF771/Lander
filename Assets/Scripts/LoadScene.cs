using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

}
