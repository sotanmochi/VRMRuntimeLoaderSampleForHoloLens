using UnityEngine;
using UnityEngine.Events;

using HoloToolkit.Unity.InputModule;
public class Interactive : MonoBehaviour, IInputClickHandler
{
	[System.Serializable]
	public class InputClickCallback : UnityEvent<RaycastHit> { }

	public bool IsEnabled = true;
	public bool UseRaycastHitInfo = false;
	public UnityEvent OnSelectEvents;
	public InputClickCallback OnSelectEventsWithRaycastHitInfo;

	public void OnInputClicked(InputClickedEventData eventData)
    {
		if (!IsEnabled)
		{
			return;
		}

		if(!UseRaycastHitInfo)
		{
			OnSelectEvents.Invoke();
		}
		else
		{
			RaycastHit hit = GazeManager.Instance.HitInfo;
			OnSelectEventsWithRaycastHitInfo.Invoke(hit);
		}
	}
}
