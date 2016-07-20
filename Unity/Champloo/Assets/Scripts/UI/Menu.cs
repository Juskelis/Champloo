using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    private Animator anim;
    private CanvasGroup cgroup;

    public bool IsOpen
    {
        get { return anim.GetBool("IsOpen"); }
        set { anim.SetBool("IsOpen", value);}
    }

    public void Awake()
    {
        anim = GetComponent<Animator>();
        cgroup = GetComponent<CanvasGroup>();

        var rect = GetComponent<RectTransform>();
        rect.offsetMax = rect.offsetMin = new Vector2(0, 0);
    }

    public void Update()
    {
        cgroup.blocksRaycasts = cgroup.interactable = anim.GetCurrentAnimatorStateInfo(0).IsName("Open");
    }
}
