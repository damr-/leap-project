using UnityEngine;
using UniRx;
using PXL.Objects.Areas;
using PXL.Utility;
using UnityEngine.UI;

namespace PXL.UI.World.Display {

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
		/// The Text Component of this GameObject
		/// </summary>
		protected Text Text {
			get { return mText ?? (mText = this.TryGetComponent<Text>()); }
		}
		private Text mText;

		protected virtual void Start() {
			DestroyArea.CurrentDestroyAmount.Subscribe(CurrentDestroyAmountChanged);
			DestroyArea.GoalReached.Subscribe(_ => HandleGoalReached());
			Text.color = DefaultColor;
			CurrentDestroyAmountChanged(0);
		}

		/// <summary>
		/// Called when the amount of objects destroyed changed
		/// </summary>
		/// <param name="newAmount">The new amount of destroyed objects in total</param>
		private void CurrentDestroyAmountChanged(int newAmount) {
			Text.text = newAmount + "/" + DestroyArea.WinDestroyAmount;
		}

		/// <summary>
		/// Called when the necessary amount of objects have been destroyed
		/// </summary>
		private void HandleGoalReached() {
			Text.color = EnoughColor;
		}

	}

}