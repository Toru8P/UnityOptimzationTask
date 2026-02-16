using System;
using UnityEngine;
using UnityEngine.Events;

public class FireHazard : MonoBehaviour
{
    public event UnityAction<FireEnteredEventArgs> onCharacterEnteredAction;

    [HideInInspector] public FireHazardScriptableObject fireHazardData;

    [SerializeField]
    private UnityEvent<FireEnteredEventArgs> onCharacterEntered = new UnityEvent<FireEnteredEventArgs>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("PlayerCharacter")) return;

        var controller = other.GetComponent<PlayerCharacterController>();
        if (controller == null) return;

        FireEnteredEventArgs args = new FireEnteredEventArgs
        {
            damageDealt = fireHazardData != null ? fireHazardData.GetRandomFireDamage() : 0,
            targetCharacterController = controller
        };
        onCharacterEntered?.Invoke(args);
        onCharacterEnteredAction?.Invoke(args);
    }
}

public class FireEnteredEventArgs
{
    public int damageDealt;
    public PlayerCharacterController targetCharacterController;
}
