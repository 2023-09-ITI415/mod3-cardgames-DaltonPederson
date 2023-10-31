using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void LoadMod1Scene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
