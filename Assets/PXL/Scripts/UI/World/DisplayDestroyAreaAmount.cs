using UnityEngine;
using UniRx;
using PXL.Objects.Areas;
using PXL.Utility;
using UnityEngine.UI;

namespace PXL.UI.World {

	[RequireComponent(typeof(Text))]
	public class DisplayDestroyAreaAmount : MonoBehaviour {

		/// <summary>
		/// The referenced DestroyArea
		/// </summary>
		public DestroyArea DestroyArea;

		/// <summary>
		/// Usual color of the text
		/// </summary>
		public Color DefaultColor = Color.black;

		/// <summary>
		/// Color for the text when there's enough objects collected
		/// </summary>
		public Color EnoughColor = Color.green;

		/// <summary>
		/// This object's Text component
		/// </summary>
		private Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		private void Start() {
			DestroyArea.CurrentDestroyAmount.Subscribe(CurrentDestroyAmountChanged);
			DestroyArea.GoalReached.Subscribe(_ => HandleGoalReached());
			Text.color = DefaultColor;
		}

		private void CurrentDestroyAmountChanged(int newAmount) {
			Text.text = newAmount + "/" + DestroyArea.WinDestroyAmount;
		}

		private void HandleGoalReached() {
			Text.color = EnoughColor;
		}

	}

}