using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour {

    public void LoadLevel(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
