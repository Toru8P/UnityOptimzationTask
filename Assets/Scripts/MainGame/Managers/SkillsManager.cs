using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SkillsManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] [Min(1)] private int skillSlotCount = 3;
    [SerializeField] private PlayerCharacterController playerForSkillEffects;
    [SerializeField] private UnityEvent<int> onSkillUsed;

    [Header("Debug")]
    [SerializeField] private bool logSkillInput = false;

    private List<SkillButtonUI> _registeredButtons = new List<SkillButtonUI>();
    private InputAction _skill1Action;
    private InputAction _skill2Action;
    private InputAction _skill3Action;

    private void Awake()
    {
        _skill1Action = new InputAction(binding: "<Keyboard>/digit1");
        _skill2Action = new InputAction(binding: "<Keyboard>/digit2");
        _skill3Action = new InputAction(binding: "<Keyboard>/digit3");
        _skill1Action.Enable();
        _skill2Action.Enable();
        _skill3Action.Enable();
    }

    private void OnDestroy()
    {
        _skill1Action?.Dispose();
        _skill2Action?.Dispose();
        _skill3Action?.Dispose();
    }

    private void Start()
    {
        if (playerForSkillEffects != null)
        {
            onSkillUsed.AddListener(playerForSkillEffects.OnSkillUsed);
#if UNITY_EDITOR
            if (logSkillInput)
                Debug.Log("[SkillsManager] Player For Skill Effects assigned: healing on keys 1/2/3 and UI buttons.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            if (logSkillInput)
                Debug.LogWarning("[SkillsManager] Player For Skill Effects is None — assign player in Inspector for skill effects (heal).");
#endif
        }
        StartCoroutine(EnsureButtonsRegisteredNextFrame());
    }

    private IEnumerator EnsureButtonsRegisteredNextFrame()
    {
        yield return null;
        if (_registeredButtons.Count > 0)
        {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log($"[SkillsManager] Buttons already registered by UIManager: {_registeredButtons.Count}");
#endif
            yield break;
        }

        List<SkillButtonUI> list = null;
        if (uiManager != null)
            list = uiManager.GetCachedSkillButtonsList();
        if (list != null && list.Count > 0)
        {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log($"[SkillsManager] Registering {list.Count} buttons from UIManager cache.");
#endif
            RegisterSkillButtons(list);
            yield break;
        }

        var found = FindObjectsByType<SkillButtonUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (found != null && found.Length > 0)
        {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log($"[SkillsManager] Fallback: found {found.Length} SkillButtonUI in scene, registering.");
#endif
            list = found.OrderBy(s => s.transform.GetSiblingIndex()).ToList();
            for (int i = 0; i < list.Count; i++)
                list[i].SetSkillIndex(i);
            RegisterSkillButtons(list);
        }
        else
        {
#if UNITY_EDITOR
            if (logSkillInput)
                Debug.LogWarning("[SkillsManager] No SkillButtonUI found in scene. Assign UIManager (and Skills Manager in UIManager) and Skills Holder, or add SkillButtonUI to skill buttons.");
#endif
        }
    }

    public void RegisterSkillButtons(List<SkillButtonUI> buttons)
    {
        if (buttons == null) return;
        _registeredButtons.Clear();
        for (int i = 0; i < buttons.Count; i++)
        {
            SkillButtonUI btn = buttons[i];
            if (btn == null) continue;
            btn.AddSkillUsedListener(UseSkill);
            _registeredButtons.Add(btn);
        }
#if UNITY_EDITOR
        if (logSkillInput)
            Debug.Log($"[SkillsManager] Registered {_registeredButtons.Count} skill buttons (keys 1/2/3 and UI will call UseSkill).");
#endif
    }

    public void UseSkill(int index)
    {
        if (index < 0 || index >= skillSlotCount) return;
#if UNITY_EDITOR
        if (logSkillInput)
            Debug.Log($"[SkillsManager] UseSkill({index}) — invoking On Skill Used (player set: {playerForSkillEffects != null}).");
#endif
        onSkillUsed?.Invoke(index);
    }

    private void Update()
    {
        bool key1 = _skill1Action != null && _skill1Action.triggered;
        bool key2 = _skill2Action != null && _skill2Action.triggered;
        bool key3 = _skill3Action != null && _skill3Action.triggered;
        if (!key1 && !key2 && !key3 && Keyboard.current != null)
        {
            key1 = Keyboard.current.digit1Key.wasPressedThisFrame;
            key2 = Keyboard.current.digit2Key.wasPressedThisFrame;
            key3 = Keyboard.current.digit3Key.wasPressedThisFrame;
        }
        if (key1) {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log("[SkillsManager] Key 1 pressed.");
#endif
            UseSkill(0);
        }
        else if (key2) {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log("[SkillsManager] Key 2 pressed.");
#endif
            UseSkill(1);
        }
        else if (key3) {
#if UNITY_EDITOR
            if (logSkillInput) Debug.Log("[SkillsManager] Key 3 pressed.");
#endif
            UseSkill(2);
        }
    }
}
