using System;
using MainGame.Characters;
using TMPro;
using UnityEngine;

namespace MainGame.Managers
{
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI hpText;
    
        [SerializeField] private PlayerCharacterController bobby;
        [SerializeField] private GameObject skillsHolder;

        public void RefreshHPText(int newHP)
        {
            hpText.text = newHP.ToString();
        }

        private void Awake()
        {
            hpText.text = bobby.Hp.ToString();
        }

        private void OnEnable()
        {
            bobby.SubscribeToTakeDamageEvent(RefreshHPText);
        }

        private void OnDisable()
        {
            bobby.UnsubscribeToTakeDamageEvent(RefreshHPText);
        }

        private void Update()
        {
            SkillButtonUI[] _skillsButtonUI = skillsHolder.GetComponentsInChildren<SkillButtonUI>();

            for (int i = 0; i < _skillsButtonUI.Length; i++)
            {
                _skillsButtonUI[i].skillIcon.sprite =  _skillsButtonUI[i].skillIcons[i];
                _skillsButtonUI[i].skillNameText.text = "Skill " + (i + 1);
            }
        }
    }
}
