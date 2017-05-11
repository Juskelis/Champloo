using UnityEngine;

public class ActivateChildren : MonoBehaviour
{
    public void Activate()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ActivateAfter(float time)
    {
        Invoke("Activate", time);
    }
}
