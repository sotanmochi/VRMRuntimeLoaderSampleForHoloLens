using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
#endif

public class VRMRuntimeLoaderUsingFilePicker : MonoBehaviour
{
	void Start ()
	{
#if !UNITY_EDITOR && UNITY_WSA_10_0
		Debug.Log("***********************************");
		Debug.Log("File Picker start.");
		Debug.Log("***********************************");

		UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
		{
			var filepicker = new FileOpenPicker();
			filepicker.FileTypeFilter.Add("*");
			filepicker.FileTypeFilter.Add(".vrm");

			var file = await filepicker.PickSingleFileAsync();
			UnityEngine.WSA.Application.InvokeOnAppThread(() => 
			{
				Debug.Log("***********************************");
				string name = (file != null) ? file.Name : "No data";
				Debug.Log("Name: " + name);
				Debug.Log("***********************************");
				string path = (file != null) ? file.Path : "No data";
				Debug.Log("Path: " + path);
				Debug.Log("***********************************");

				StartCoroutine(LoadVrmCoroutine(path));

			}, false);
		}, false);

		Debug.Log("***********************************");
		Debug.Log("File Picker end.");
		Debug.Log("***********************************");
#endif
	}

	IEnumerator LoadVrmCoroutine(string path)
	{
		var www = new WWW("file://" + path);
		yield return www;
		VRM.VRMImporter.LoadVrmAsync(www.bytes, OnLoaded);
	}

	void OnLoaded(GameObject go)
	{

	}
}
