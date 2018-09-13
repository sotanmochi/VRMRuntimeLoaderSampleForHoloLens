using System.Collections;
using UnityEngine;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
#endif

public class VRMRuntimeLoaderUsingFilePicker : MonoBehaviour
{
	public GameObject VRMRoot;

	void Start ()
	{
#if !UNITY_EDITOR && UNITY_WSA_10_0

		UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
		{
			var openPicker = new FileOpenPicker();
			openPicker.SuggestedStartLocation = PickerLocationId.Objects3D;
			openPicker.FileTypeFilter.Add(".vrm");

			var file = await openPicker.PickSingleFileAsync();
			UnityEngine.WSA.Application.InvokeOnAppThread(() => 
			{
				if(file != null)
				{
					StartCoroutine(LoadVrmCoroutine(file.Path));
				}
			}, false);
		}, false);

#elif UNITY_EDITOR

		string path = Application.dataPath + "/Models/" + "default.vrm";
		StartCoroutine(LoadVrmCoroutine(path));

#endif
	}

	IEnumerator LoadVrmCoroutine(string path)
	{
		var www = new WWW("file://" + path);
		yield return www;
		VRM.VRMImporter.LoadVrmAsync(www.bytes, OnLoaded);
	}

	void OnLoaded(GameObject vrm)
	{
		if(VRMRoot != null)
		{
			vrm.transform.SetParent(VRMRoot.transform, false);
		}
	}
}
