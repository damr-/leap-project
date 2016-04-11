using System;
using PXL.Gamemodes;
using PXL.Objects.Areas;
using PXL.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PXL.UI.World {

	[RequireComponent(typeof(CheckingStackArea))]
	public class DisplayStackAreaStatus : MonoBehaviour {

		/// <summary>
		/// The StackArea component of this object
		/// </summary>
		private CheckingStackArea CheckingStackArea {
			get { return mCheckingStackArea ?? (mCheckingStackArea = this.TryGetComponent<CheckingStackArea>()); }
		}

		private CheckingStackArea mCheckingStackArea;

		/// <summary>
		/// Checkmarks to indicate the number of correct elements
		/// </summary>
		public Image[] ObjectsCheckMarks;

		/// <summary>
		/// Checkmarks to indicate the stacked correctly check process
		/// </summary>
		public Image[] CheckCheckMarks;

		/// <summary>
		/// The progressbar which displays the progress of checking the stacked objects
		/// </summary>
		public Scrollbar CheckProgressbar;

		/// <summary>
		/// Text to show the current status of the checks
		/// </summary>
		public Text StatusText;

		/// <summary>
		/// How often per second the check is executed
		/// </summary>
		private const float StackedCorrectlyCheckFrequency = 1f;

		/// <summary>
		/// The counter of the current finished amount of checks
		/// </summary>
		private int lastChecksCounter;

		/// <summary>
		/// The last time the stacked objects have been checked
		/// </summary>
		private float lastCheckTime;

		/// <summary>
		/// Success color
		/// </summary>
		private readonly Color success = new Color(140 / 255f, 1f, 130 / 255f, 1f);

		/// <summary>
		/// Failure color
		/// </summary>
		private readonly Color failure = new Color(1f, 140 / 255f, 130 / 255f, 1f);

		/// <summary>
		/// The index of the 
		/// </summary>
		private int incorrectIndex;

		private void Start() {
			CheckProgressbar.AssertNotNull();
			StatusText.AssertNotNull();
			if (ObjectsCheckMarks.Length != CheckingStackArea.RequiredObjectsAmount)
				throw new MissingReferenceException("Not enough check marks assigned!");

			CheckingStackArea.AreaStatus.Subscribe(HandleStatusChange);
		}

		/// <summary>
		/// Called when the status of the StackArea changes
		/// </summary>
		private void HandleStatusChange(StackArea.Status status) {
			incorrectIndex = 0;
			switch (status) {
				case StackArea.Status.NotStationary:
					CancelChecks("objects not stationary");
					break;
				case StackArea.Status.StackedIcorrectly:
					var o = CheckingStackArea.IncorrectObject;
					var i = CheckingStackArea.SortedObjects.IndexOf(o);
					CancelChecks("object " + (i + 1) + "stacked incorrectly");
					incorrectIndex = i;
					break;
				case StackArea.Status.NotEnoughObjects:
					CancelChecks("not enough objects");
					break;
			}
		}

		private void Update() {
			if (GameMode.GameOver)
				return;

			foreach (var i in ObjectsCheckMarks)
				i.color = Color.white;

			for (var i = 0; i < Mathf.Clamp(CheckingStackArea.SortedObjects.Count, 0, ObjectsCheckMarks.Length); i++) {
				var color = (incorrectIndex >= 0 && i == incorrectIndex) ? failure : success;
				ObjectsCheckMarks[i].color = color;
			}

			if (CheckingStackArea.AreaStatus != StackArea.Status.Checking)
				return;

			CheckProgressbar.size = Time.time.Remap(lastCheckTime, lastCheckTime + 1 / StackedCorrectlyCheckFrequency, 0, 1);

			if (!(Time.time - lastCheckTime > 1 / StackedCorrectlyCheckFrequency))
				return;

			lastCheckTime = Time.time;

			if (lastChecksCounter == 0) {
				StatusText.text = "checking...";
				foreach (var checkMark in CheckCheckMarks)
					checkMark.gameObject.SetActive(true);
			}

			CheckCheckMarks[lastChecksCounter++].color = success;
			if (lastChecksCounter <= CheckCheckMarks.Length - 1)
				return;

			HandleGameWon();
		}

		/// <summary>
		/// Resets the current progress in checking the objects inside the StackArea
		/// </summary>
		/// <param name="statusText"></param>
		private void CancelChecks(string statusText) {
			CheckProgressbar.size = 0f;
			lastChecksCounter = 0;
			lastCheckTime = 0;
			if (StatusText.text != statusText)
				StatusText.text = statusText;
			foreach (var t in CheckCheckMarks)
				t.color = Color.white;
			foreach (var checkMark in CheckCheckMarks)
				checkMark.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called when the objects of the StackArea have been stacked correctly and checked a certain amount of times
		/// </summary>
		private void HandleGameWon() {
			lastChecksCounter = 0;
			CheckingStackArea.SetAreaStackedCorrectly();
		}

	}

}