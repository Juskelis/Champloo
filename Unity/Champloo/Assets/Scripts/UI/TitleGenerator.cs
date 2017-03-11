using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Events;

//this whole system is basically ripped from videogamena.me

public class TitleGenerator : MonoBehaviour
{
    private static string TITLE = "";

    [SerializeField]
    private TextAsset gameNames;

    [SerializeField]
    private bool allowSimilarWords = false;

    [SerializeField]
    private bool generateAfterInterval = false;

    [SerializeField]
    private float interval = 10f;

    [SerializeField]
    private string[] secretPhraseList;

    [SerializeField]
    private UnityEvent secretEvent;

    private List<List<string>> word_list;

    private Text toChange;

    private void Awake()
    {
        toChange = GetComponent<Text>();
    }

    private void Start()
    {
        if (TITLE.Length > 0)
        {
            toChange.text = TITLE;
            return;
        }
        BuildList();
        CreateTitle();
        TITLE = toChange.text;
    }

    private bool CheckSecret(string title)
    {
        return secretPhraseList.Contains(title);
    }

    [ContextMenu("Create Title")]
    private void CreateTitle()
    {
        toChange.text = Generate();
        if (generateAfterInterval) Invoke("CreateTitle", interval);
        if (CheckSecret(toChange.text))
        {
            secretEvent.Invoke();
        }
    }

    private void BuildList()
    {
        string[] words = gameNames.text.Split('\n');
        word_list = new List<List<string>>();
        word_list.Add(new List<string>());

        int word_list_index = 0;

        foreach(string word in words)
        {
            if(word.Contains("----"))
            {
                word_list_index++;
                word_list.Add(new List<string>());
            }
            else
            {
                word_list[word_list_index].Add(word);
            }
        }
    }

    public string Generate()
    {
        string first_word = word_list[0][Random.Range(0, word_list[0].Count - 1)];
        string second_word = "";
        string third_word = "";

        List<string> bad_words = new List<string>();

        if(first_word.IndexOf('^') >= 0)
        {
            if(!allowSimilarWords)
            {
                bad_words.AddRange(first_word.Split('^')[1].Split('|'));
            }
            first_word = first_word.Split('^')[0];
        }

        bool second_word_bad = true;
        while(second_word_bad)
        {
            second_word = word_list[1][Random.Range(0, word_list[1].Count - 1)];
            if(second_word.IndexOf('^') >= 0)
            {
                if(!allowSimilarWords)
                {
                    bad_words.AddRange(second_word.Split('^')[1].Split('|'));
                }
                second_word = second_word.Split('^')[0];
            }

            if (second_word.Equals(first_word)) continue;

            if (bad_words.Contains(second_word)) continue;

            second_word_bad = false;
        }

        bool third_word_bad = true;
        while(third_word_bad)
        {
            third_word = word_list[2][Random.Range(0, word_list[2].Count - 1)];
            if (third_word.IndexOf('^') >= 0)
            {
                if (!allowSimilarWords)
                {
                    bad_words.AddRange(third_word.Split('^')[1].Split('|'));
                }
                third_word = third_word.Split('^')[0];
            }

            if (third_word.Equals(first_word )|| third_word.Equals(second_word)) continue;

            if (bad_words.Contains(third_word)) continue;

            third_word_bad = false;
        }

        return first_word + " " + second_word + " " + third_word;
    }

}
