using UnityEngine;
using System.Collections;
using GameInput;
using UnityEngine.Events;

[System.Serializable]
public class InteractiveEvent : UnityEvent<object>
{
    public UnityEvent<object> Event;
}

public class InteractiveTrigger : MonoBehaviour
{
    [System.Serializable]
    public class InteractionInfo
    {
        public Button ActionButton;
        public string CalloutText;
        public InteractiveEvent Action;
    }

    [SerializeField] protected Button _actionButton;
    [SerializeField] protected string _calloutText;
    [SerializeField] protected InteractiveEvent _action;

    protected InteractionInfo _info;

    protected virtual void Start()
    {
        _info = new InteractionInfo();

        _info.ActionButton = _actionButton;
        _info.CalloutText = _calloutText;
        _info.Action = _action;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Player p = other.GetComponent<Player>();
            if (p != null)
            {
                p.AcceptInteractiveInfo(true, _info);
            }
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Player p = other.GetComponent<Player>();
            if (p != null)
            {
                p.AcceptInteractiveInfo(false, null);
            }
        }
    }
}
