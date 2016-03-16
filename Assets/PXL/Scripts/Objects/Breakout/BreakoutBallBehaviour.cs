using PXL.Utility;
using UnityEngine;

namespace PXL.Objects.Breakout {

	[RequireComponent(typeof(Rigidbody))]
	public class BreakoutBallBehaviour : MonoBehaviour {

		/// <summary>
		/// Speed of the ball
		/// </summary>
		public float speed = 10f;

		private Vector3 oldVelocity;

		private Rigidbody Rigidbody {
			get { return mRigidbody ?? (mRigidbody = this.TryGetComponent<Rigidbody>()); }
		}
		private Rigidbody mRigidbody;

		private void Start() {
			SetupRigidbody();
			transform.position = new Vector3(transform.position.x, -0.245f, transform.position.z);
			Rigidbody.velocity = Vector3.forward * speed;
		}

		private void LateUpdate() {
			oldVelocity = Rigidbody.velocity;
		}

		private void SetupRigidbody() {
			Rigidbody.isKinematic = false;
			Rigidbody.drag = 0f;
			Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		}

		private void OnCollisionEnter(Collision collision) {
			var contact = collision.contacts[0];
			var reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);
			Rigidbody.velocity = reflectedVelocity;
		}
	}

}