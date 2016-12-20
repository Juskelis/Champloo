using System;
using UnityEngine;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine.Events;

[Serializable]
public class VotingEvent : UnityEvent<VotingManager.Option> { }

public class VotingManager : MonoBehaviour
{
    
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
        //on an individual vote
        //electedOption = TallyVotes();
    }

    public void AllSelectedCallback()
    {
        print("Checking active status: " + gameObject.activeInHierarchy + " " + gameObject.activeSelf);
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
        {
            print("Not active");
            return;
        }

        //do things
        electedOption = TallyVotes();
        OnElected.Invoke(electedOption);
    }
}
