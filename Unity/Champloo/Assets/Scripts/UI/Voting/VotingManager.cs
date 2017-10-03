using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class VotingEvent : UnityEvent<VotingManager.Option> { }

public class VotingManager : MonoBehaviour
{
    public Outline selectedOutline;
    
    [Serializable]
    public class Option
    {
        public MultiplayerSelectable selectable;
        public int rating;
        [Tooltip("Enable to have the option take priority over others, even if only one person selects it")]
        public bool forceSelect;

        public int voteCount
        {
            get { return selectable.ControllersSelecting(); }
        }
    }

    public Option[] options;
    private Option electedOption;

    public VotingEvent OnElected;

    public UnityEvent OnVotingTimeout;

    public bool hasVotingTimeout = false;

    public float votingTimeout = 10f;

    public float VotingTimeLeft
    {
        get { return votingStarted && hasVotingTimeout ? votingTimeout - (Time.time - votingStartTime) : 0f; }
    }

    private bool votingStarted = false;
    private float votingStartTime = 0;

    public void Reset()
    {
        CancelInvoke("ForceVote");
        votingStarted = false;
    }

    private Option TallyVotes()
    {
        Option mostVotes = null;
        Option ret = null;
        foreach (Option option in options)
        {
            if (!option.forceSelect)
            {
                //normal tallying
                if (mostVotes == null && option.voteCount > 0)
                {
                    mostVotes = option;
                } else if (mostVotes != null && mostVotes.voteCount*mostVotes.rating < option.voteCount*option.rating)
                {
                    mostVotes = option;
                }
            }
            else
            {
                //force tallying
                if (ret == null && option.selectable.ControllersSelecting() > 0)
                {
                    ret = option;
                } else if (ret != null && ret.voteCount < option.voteCount*option.rating)
                {
                    ret = option;
                }
            }
        }

        return ret ?? mostVotes;
    }

    public void OnVote(MultiplayerSelectable selected, MultiplayerUIController controller)
    {
        if (!votingStarted && hasVotingTimeout)
        {
            votingStarted = true;
            votingStartTime = Time.time;
            Invoke("ForceVote", votingTimeout);
        }
        //on an individual vote
        Option prevElectedOption = electedOption;
        electedOption = TallyVotes();
        if(prevElectedOption != null)
        {
            //visuals for winner change
            Destroy(prevElectedOption.selectable.GetComponent<Outline>());
        }
        Outline o = electedOption.selectable.gameObject.AddComponent<Outline>();
        o.effectColor = selectedOutline.effectColor;
        o.effectDistance = selectedOutline.effectDistance;
        OnElected.Invoke(electedOption);
    }

    public void AllSelectedCallback()
    {
        print("Checking active status: " + gameObject.activeInHierarchy + " " + gameObject.activeSelf);
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
        {
            print("Not active");
            return;
        }

        CancelInvoke("ForceVote");

        //do things
        electedOption = TallyVotes();
        OnElected.Invoke(electedOption);
    }

    private void ForceVote()
    {
        AllSelectedCallback();
        OnVotingTimeout.Invoke();
    }
}
