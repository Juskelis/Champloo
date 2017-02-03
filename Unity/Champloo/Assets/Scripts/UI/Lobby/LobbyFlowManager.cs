using UnityEngine;
using System.Collections;

public class LobbyFlowManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] activationOrder;

    int current = 0;

    public void OnEnable()
    {
        for (int i = 0; i < activationOrder.Length; i++)
        {
            activationOrder[i].SetActive(false);
        }
        activationOrder[current].SetActive(true);
    }

    public void Next()
    {
        activationOrder[current].SetActive(false);
        current = Mathf.Min(activationOrder.Length - 1, current + 1);
        activationOrder[current].SetActive(true);
    }

    public void Previous()
    {
        activationOrder[current].SetActive(false);
        current = Mathf.Max(0, current - 1);
        activationOrder[current].SetActive(true);
    }
}
