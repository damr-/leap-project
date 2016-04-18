using PXL.Objects.Breakout;

namespace PXL.UI.Admin {

	public class BallSpeedChanger : PropertyChanger {

		/// <summary>
		/// How much the speed changes when it's in/decreased
		/// </summary>
		private const float SpeedChangeAmount = 0.1f;
		
		public override void NextValue() {
			SetBallSpeed(BreakoutBallBehaviour.Speed + SpeedChangeAmount);
		}

		public override void PreviousValue() {
			SetBallSpeed(BreakoutBallBehaviour.Speed - SpeedChangeAmount);
		}
		
		protected override void Start() {
			base.Start();
			ResetSpeed();
		}

		/// <summary>
		/// Sets the ball speed to the default speed
		/// </summary>
		public void ResetSpeed() {
			SetBallSpeed(BreakoutBallBehaviour.DefaultSpeed);
		}

		/// <summary>
		/// Sets the speed of the ball to the given value, if valid
		/// </summary>
		private void SetBallSpeed(float newSpeed) {
			if (newSpeed < BreakoutBallBehaviour.MinSpeed || newSpeed > BreakoutBallBehaviour.MaxSpeed)
				return;
			BreakoutBallBehaviour.Speed = newSpeed;
			SetText(newSpeed);
		}

		/// <summary>
		/// Sets the text of the text component to the given ball speed
		/// </summary>
		private void SetText(float speed) {
			if(PropertyText != null)
				PropertyText.text = speed.ToString("0.00");
		}
	}

}