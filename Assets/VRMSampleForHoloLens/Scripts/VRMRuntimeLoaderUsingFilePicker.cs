using System.Collections;
using UnityEngine;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
#endif

using HoloToolkit.Unity.InputModule;

public class VRMRuntimeLoaderUsingFilePicker : MonoBehaviour
{
	public GameObject VRMRoot;

	UniHumanoid.HumanPoseTransfer m_target;
	UniHumanoid.HumanPoseTransfer m_source;

	public void UpdateVRMRootPosition(RaycastHit hitInfo)
	{
		if(Vector3.Dot(hitInfo.normal, Vector3.up) > 0.95f)
		{
			VRMRoot.transform.position = hitInfo.point;

			Vector3 direction = Camera.main.transform.position - hitInfo.point;
			direction = Vector3.Cross(Vector3.Cross(transform.up, direction), transform.up); // up方向成分の除去
			VRMRoot.transform.forward = direction;
		}
	}

	public void LoadVrmUsingFilePicker()
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

#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

		var path = VRM.FileDialogForWindows.FileDialog("open VRM", ".vrm");
		if (!string.IsNullOrEmpty(path))
		{
			StartCoroutine(LoadVrmCoroutine(path));
		}

#elif UNITY_EDITOR

		string path = Application.dataPath + "/Models/" + "default-vrm.vrm";
		StartCoroutine(LoadVrmCoroutine(path));

#endif
	}

	public void LoadBvhUsingFilePicker()
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0

		UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
		{
			var openPicker = new FileOpenPicker();
			openPicker.SuggestedStartLocation = PickerLocationId.Objects3D;
			openPicker.FileTypeFilter.Add(".bvh");

			var file = await openPicker.PickSingleFileAsync();
			UnityEngine.WSA.Application.InvokeOnAppThread(() => 
			{
				if(file != null)
				{
					StartCoroutine(LoadBvhCoroutine(file.Path));
				}
			}, false);
		}, false);

#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

		var path = VRM.FileDialogForWindows.FileDialog("open BVH", ".bvh");
		if (!string.IsNullOrEmpty(path))
		{
			StartCoroutine(LoadBvhCoroutine(path));
		}

#elif UNITY_EDITOR

		string path = Application.dataPath + "/Models/" + "default-bvh.bvh";
		StartCoroutine(LoadBvhCoroutine(path));

#endif
	}

	private IEnumerator LoadVrmCoroutine(string path)
	{
		var www = new WWW("file://" + path);
		yield return www;
		VRM.VRMImporter.LoadVrmAsync(www.bytes, OnLoaded);
	}

	private void OnLoaded(GameObject vrm)
	{
		if(VRMRoot != null)
		{
			vrm.transform.SetParent(VRMRoot.transform, false);
			
			// add motion
            var humanPoseTransfer = vrm.AddComponent<UniHumanoid.HumanPoseTransfer>();
            if (m_target != null)
            {
                GameObject.Destroy(m_target.gameObject);
            }
            m_target = humanPoseTransfer;

			EnableBvh();
		}
	}

	IEnumerator LoadBvhCoroutine(string path)
	{
		var www = new WWW("file://" + path);
		yield return www;

		var context = new UniHumanoid.ImporterContext
		{
			Path = path,
			Source = www.text
		};
		ModifiedBvhImporter.Import(context);

		if (m_source != null)
		{
			GameObject.Destroy(m_source.gameObject);
		}
		m_source = context.Root.GetComponent<UniHumanoid.HumanPoseTransfer>();

        m_source.GetComponent<Renderer>().enabled = false;

		EnableBvh();
	}

	void EnableBvh()
	{
		if (m_target != null)
		{
			m_target.Source = m_source;
			m_target.SourceType = UniHumanoid.HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;
		}
	}
}
