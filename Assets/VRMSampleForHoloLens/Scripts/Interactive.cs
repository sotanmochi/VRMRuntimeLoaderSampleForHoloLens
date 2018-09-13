using UnityEngine;
using UnityEngine.Events;

using HoloToolkit.Unity.InputModule;
public class Interactive : MonoBehaviour, IInputClickHandler
{
	public bool IsEnabled = true;
	public UnityEvent OnSelectEvents;

	public void OnInputClicked(InputClickedEventData eventData)
    {
		if (!IsEnabled)
		{
			return;
		}
		OnSelectEvents.Invoke();
	}
}
