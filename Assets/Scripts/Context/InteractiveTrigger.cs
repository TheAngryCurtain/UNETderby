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
    // idea:
    // interactive triggers hold data on what button needs to be pressed, callout text, and the action that needs to happen when that button is pressed
    
    [System.Serializable]
    public class InteractionInfo
    {
        public Button ActionButton;
        public string CalloutText;
        public InteractiveEvent Action;
    }

    [SerializeField] private InteractionInfo _info;

    public void OnTriggerEnter(Collider other)
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

    public void OnTriggerExit(Collider other)
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
