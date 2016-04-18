using UnityEngine;
using UniRx;
using PXL.Objects.Areas;

namespace PXL.UI.World.Display {

	public class DisplayDestroyAreaAmount : DisplayTextBase {

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

		protected override void Start() {
			base.Start();

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