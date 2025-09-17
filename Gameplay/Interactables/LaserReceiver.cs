using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    [Header("Activation Events")]
    [SerializeField] private List<UnityEvent> onActivatedEvents = new List<UnityEvent>();

    [Header("Deactivation Events")]
    [SerializeField] private List<UnityEvent> onDeactivatedEvents = new List<UnityEvent>();

    private bool isActive = false;

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            foreach (var e in onActivatedEvents)
                e?.Invoke();
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            foreach (var e in onDeactivatedEvents)
                e?.Invoke();
        }
    }
}
