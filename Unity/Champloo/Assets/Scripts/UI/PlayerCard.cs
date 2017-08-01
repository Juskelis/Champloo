using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField]
    private int playerNumber;
    public int PlayerNumber
    {
        get { return playerNumber; }

        set
        {
            playerNumber = value;
            UpdateChildrenPlayerNumbers();
        }
    }

    void UpdateChildrenPlayerNumbers()
    {
        foreach (PlayerScoreCard card in GetComponentsInChildren<PlayerScoreCard>(true))
        {
            card.playerNumber = playerNumber;
        }

        foreach (WeaponIndicator indicator in GetComponentsInChildren<WeaponIndicator>(true))
        {
            indicator.PlayerNumber = playerNumber;
        }
    }
}
