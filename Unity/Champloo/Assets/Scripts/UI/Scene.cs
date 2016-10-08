using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Scene : MonoBehaviour {

    public void LoadLevel(string s)
    {
        //SceneManager.LoadScene(s);
        NetworkManager.singleton.ServerChangeScene(s);

    }

    public void Quit()
    {
        Application.Quit();
    }
}

