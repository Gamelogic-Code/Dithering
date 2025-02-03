// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <inheritdoc />
	/// <summary>
	/// A component that makes it easy to take screenshots, usually for development purposes.
	/// </summary>
	[AddComponentMenu("Gamelogic/Extensions/ScreenshotTaker")]
	[ExecuteInEditMode]
	public sealed class ScreenshotTaker : Singleton<ScreenshotTaker>
	{
		#region Configuration

		[Tooltip("The camera from where the screenshot will be taken.")]
		[SerializeField]
		private Camera screenshotCamera = null;
		
		[Tooltip("The key to use for taking a screenshot.")]
		[SerializeField]
		private KeyCode screenshotKey = KeyCode.Q;

		[Tooltip("The scale at which to take the screen shot.")]
		[ValidatePositive]
		[SerializeField]
		private int scale = 1;

		[Tooltip("The fist part of the file name")]
		[SerializeField]
		[ValidateMatchRegularExpression(@"^[^<>:""/\\|?*\x00-\x1F]+$")]
		private string fileNamePrefix = "screen_";

		[Tooltip("Set this to true to have screenshots taken periodically and specify the interval in seconds.")]
		[SerializeField]
		private OptionalFloat automaticScreenshotInterval = new OptionalFloat { UseValue = false, Value = 60f };

		[Tooltip("Objects to enable when taking a screenshot.")]
		[SerializeField]
		private GameObject[] screenshotOnlyObjects = null;

		[Tooltip("Objects to disable when taking a screenshot.")]
		[SerializeField]
		private GameObject[] dirtyObjects = Array.Empty<GameObject>();

		private Dictionary<GameObject, bool> stateOfScreenshotOnlyObjects;
		private Dictionary<GameObject, bool> stateOfDirtyObjects;

		#endregion

		#region Unity Messages

		public void Start()
		{
			if (Application.isPlaying && automaticScreenshotInterval.UseValue)
			{
				if (dirtyObjects.Length > 0)
				{
					InvokeRepeating(TakeCleanImpl, automaticScreenshotInterval.Value, automaticScreenshotInterval.Value);
				}
				else
				{
					InvokeRepeating(TakeImpl, automaticScreenshotInterval.Value, automaticScreenshotInterval.Value);
				}
			}
		}

		public void Update()
		{
			if (Input.GetKeyDown(screenshotKey))
			{
				if (dirtyObjects.Length > 0)
				{
					TakeClean();
				}
				else
				{
					Take();
				}
			}
		}

		#endregion

		#region Public Methods

		[InspectorButton]
		public static void Take()
		{
			Instance.TakeImpl();
		}

		[InspectorButton]
		public static void TakeClean()
		{
			Instance.TakeCleanImpl();
		}

		public static Texture2D TakeTexture()
		{
			return Instance.TakeTexture__();
		}

		#endregion

		#region Implementation

		private void TakeCleanImpl()
		{
			StartCoroutine(TakeCleanEnumerator());
		}

		private IEnumerator TakeCleanEnumerator()
		{
			SetScreenshotDirtyObject();
			SetScreenshotOnlyObjects();

			yield return new WaitForEndOfFrame();

			TakeImpl();

			yield return new WaitForEndOfFrame();

			RestoreScreenshotOnlyObjects();
			RestoreScreenshotDirtyObjects();
		}

		private void TakeImpl()
		{
			var path = fileNamePrefix + DateTime.Now.Ticks + ".png";
			ScreenCapture.CaptureScreenshot(path, scale);
		}

		private Texture2D TakeTexture__()
		{
			SetScreenshotOnlyObjects();
			SetScreenshotDirtyObject();

			var rt = new RenderTexture(Screen.width, Screen.height, 24);
			screenshotCamera.targetTexture = rt;
			var screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			screenshotCamera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			screenshotCamera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			screenShot.Apply();
			Destroy(rt);
#if UNITY_EDITOR
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = fileNamePrefix + DateTime.Now.Ticks + ".png";
			System.IO.File.WriteAllBytes("screenshots/" + filename, bytes);
			Debug.Log($"Took screenshot to: {filename}");
#endif

			RestoreScreenshotOnlyObjects();
			RestoreScreenshotDirtyObjects();

			return screenShot;
		}

		private void SetScreenshotOnlyObjects()
		{
			stateOfScreenshotOnlyObjects = new Dictionary<GameObject, bool>();

			foreach (var screenshotOnlyObject in screenshotOnlyObjects)
			{
				stateOfScreenshotOnlyObjects.Add(screenshotOnlyObject, screenshotOnlyObject.activeSelf);
				screenshotOnlyObject.SetActive(true);
			}
		}

		private void RestoreScreenshotOnlyObjects()
		{
			foreach (var stateOfScreenshotOnlyObject in stateOfScreenshotOnlyObjects)
			{
				stateOfScreenshotOnlyObject.Key.SetActive(stateOfScreenshotOnlyObject.Value);
			}
		}

		private void SetScreenshotDirtyObject()
		{
			stateOfDirtyObjects = new Dictionary<GameObject, bool>();

			foreach (var dirtyObject in dirtyObjects)
			{
				stateOfDirtyObjects.Add(dirtyObject, dirtyObject.activeSelf);
				dirtyObject.SetActive(false);
			}
		}

		private void RestoreScreenshotDirtyObjects()
		{
			foreach (var stateOfDirtyObject in stateOfDirtyObjects)
			{
				stateOfDirtyObject.Key.SetActive(stateOfDirtyObject.Value);
			}
		}

		#endregion
	}
}
