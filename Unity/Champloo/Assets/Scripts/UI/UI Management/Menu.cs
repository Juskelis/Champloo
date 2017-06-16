using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
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
                cgroup.blocksRaycasts = cgroup.interactable = true;
                eventSystem = EventSystem.current;
                Selectable selected = initiallySelected.GetComponent<Selectable>();
                selected.enabled = true;
                eventSystem.SetSelectedGameObject(initiallySelected);
                eventSystem.UpdateModules();
            }
        }
    }

    [SerializeField]
    private GameObject initiallySelected;

    private MultiplayerUIManager UIManager;

    private RectTransform rect;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        cgroup = GetComponent<CanvasGroup>();
        //eventSystem = FindObjectOfType<EventSystem>();

        rect = GetComponent<RectTransform>();
        rect.offsetMax = rect.offsetMin = Vector2.zero;
        UIManager = GetComponent<MultiplayerUIManager>();

        if(UIManager != null) UIManager.enabled = false;
    }

    public void Update()
    {
        cgroup.blocksRaycasts = cgroup.interactable = IsOpen;

        //sometimes network stuff is wonky so reset menu location to make sure things aren't dumb
        rect.offsetMax = rect.offsetMin = Vector2.zero;
    }
}
