using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTriggerZone : MonoBehaviour
{
    [SerializeField] private DoorController doorController;

    [SerializeField] private string triggerTag = "PlayerCharacter";

    private void Awake()
    {
        if (doorController == null)
            doorController = GetComponentInParent<DoorController>();

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (doorController == null) return;
        if (string.IsNullOrEmpty(triggerTag) || other.CompareTag(triggerTag))
            doorController.Open();
    }
}
