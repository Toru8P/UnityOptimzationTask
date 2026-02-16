using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private PlayerCharacterController bobby;
    [SerializeField] private Transform skillsHolder;
    [SerializeField] private SkillsManager skillsManager;

    private List<SkillButtonUI> _skillButtons = new List<SkillButtonUI>();

    public void RefreshHPText(int newHP)
    {
        if (hpText != null)
            hpText.text = newHP.ToString();
    }

    private void Awake()
    {
        if (bobby != null)
            bobby.onTakeDamageEventAction += RefreshHPText;
    }

    private void Start()
    {
        if (hpText != null && bobby != null)
            hpText.text = bobby.Hp.ToString();

        CacheSkillButtons();
    }

    private Transform GetSkillsHolderTransform()
    {
        GameObject found = GameObject.Find("Skills Group");
        if (found != null) return found.transform;
        found = GameObject.Find("Skills");
        if (found != null) return found.transform;
        return null;
    }

    private void CacheSkillButtons()
    {
        _skillButtons.Clear();
        Transform holder = GetSkillsHolderTransform();
        if (holder == null) return;

        SkillButtonUI[] buttons = holder.GetComponentsInChildren<SkillButtonUI>(true);
        _skillButtons.AddRange(buttons);
        for (int i = 0; i < _skillButtons.Count; i++)
        {
            var btn = _skillButtons[i];
            btn.SetSkillIndex(i);
            if (btn.skillIcons != null && i < btn.skillIcons.Length && btn.skillIcon != null)
                btn.skillIcon.sprite = btn.skillIcons[i];
            if (btn.skillNameText != null)
                btn.skillNameText.text = "Skill " + (i + 1);
        }

        if (skillsManager != null)
            skillsManager.RegisterSkillButtons(_skillButtons);
    }

    public List<SkillButtonUI> GetCachedSkillButtonsList()
    {
        return _skillButtons;
    }
}
