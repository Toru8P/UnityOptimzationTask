using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] skillIcons;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;

    [SerializeField] private int skillIndex;
    [SerializeField] private UnityEvent<int> onSkillClicked;
    [SerializeField] private bool logClicks = false;

    public void SetSkillIndex(int index)
    {
        skillIndex = index;
    }

    private Button _button;

    private void Awake()
    {
        CacheButton();
        EnsureCanReceiveClicks();
    }

    private void CacheButton()
    {
        _button = GetComponent<Button>();
        if (_button == null)
            _button = GetComponentInParent<Button>();
        if (_button == null)
            _button = GetComponentInChildren<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    private void EnsureCanReceiveClicks()
    {
        if (_button != null) return;
        var graphic = GetComponent<Graphic>();
        if (graphic != null)
            graphic.raycastTarget = true;
        else
        {
            var img = gameObject.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.01f);
            img.raycastTarget = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    private void OnClick()
    {
#if UNITY_EDITOR
        if (logClicks)
            Debug.Log($"[SkillButtonUI] Skill button {skillIndex} clicked on {name}.");
#endif
        onSkillClicked?.Invoke(skillIndex);
    }

    public void AddSkillUsedListener(UnityEngine.Events.UnityAction<int> callback)
    {
        if (callback != null)
            onSkillClicked.AddListener(callback);
    }
}
