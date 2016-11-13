using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    private Animator anim;
    private CanvasGroup cgroup;
    private EventSystem eventSystem;

    public bool IsOpen
    {
        get { return anim.GetBool("IsOpen"); }
        set
        {
            anim.SetBool("IsOpen", value);
            if (UIManager != null)
            {
                UIManager.enabled = value;
            }

            if (value && initiallySelected != null)
            {
                eventSystem = FindObjectOfType<EventSystem>();
                eventSystem.SetSelectedGameObject(initiallySelected);
            }
        }
    }

    [SerializeField]
    private GameObject initiallySelected;

    private MultiplayerUIManager UIManager;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        cgroup = GetComponent<CanvasGroup>();
        //eventSystem = FindObjectOfType<EventSystem>();

        var rect = GetComponent<RectTransform>();
        rect.offsetMax = rect.offsetMin = new Vector2(0, 0);
        UIManager = GetComponent<MultiplayerUIManager>();

        if(UIManager != null) UIManager.enabled = false;
    }

    public void Update()
    {
        cgroup.blocksRaycasts = cgroup.interactable = anim.GetCurrentAnimatorStateInfo(0).IsName("Open");
    }
}
