using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace PXL.Interaction {

	public class TrackData {
		public Grabbable Grabbable;
		public List<float> Time;
		public List<float> Speed;

		public TrackData(Grabbable grabbable, List<float> time, List<float> speed) {
			Grabbable = grabbable;
			Time = time;
			Speed = speed;
		}
	}

	public class FluencyObserver : MonoBehaviour {

		/// <summary>
		/// The HandModels in this scene, used to setup InteractionHand references
		/// </summary>
		public List<HandModel> HandModels = new List<HandModel>();

		/// <summary>
		/// The actual InteractionHand references
		/// </summary>
		protected List<InteractionHand> InteractionHands = new List<InteractionHand>();

		/// <summary>
		/// The currently observed object
		/// </summary>
		private Grabbable observedGrabbable;

		/// <summary>
		/// Speed data of the currently observed object
		/// </summary>
		private List<float> speedData;

		/// <summary>
		/// Time data of the currently observed object
		/// </summary>
		private List<float> timeData;

		/// <summary>
		/// Time the tracking started
		/// </summary>
		private float startTime;

		/// <summary>
		/// Invoked when an object is dropped and the observing is finished
		/// </summary>
		public IObservable<TrackData> FinishedObserving { get { return finishedObservingSubject; } }
		private readonly ISubject<TrackData> finishedObservingSubject = new Subject<TrackData>();

		private void Start() {
			HandModels.ForEach(i => InteractionHands.Add(i.GetComponent<InteractionHand>()));

			foreach (var hand in InteractionHands) {
				hand.ObjectGrabbed.Subscribe(grabbable => HandleGrabStateChanged(grabbable, true));
				hand.ObjectDropped.Subscribe(grabbable => HandleGrabStateChanged(grabbable, false));
				hand.ObjectMoved.Subscribe(HandleObjectMoved);
			}
		}

		/// <summary>
		/// Called when an InteractionHand grabs or drops an object
		/// </summary>
		private void HandleGrabStateChanged(Grabbable grabbable, bool grabbed) {
			if (grabbed) {
				observedGrabbable = grabbable;
				speedData = new List<float>();
				timeData = new List<float>();
				startTime = Time.time;
			}
			else {
				if (timeData.Count <= 2)
					return;

				var trackData = new TrackData(observedGrabbable, timeData, speedData);
				finishedObservingSubject.OnNext(trackData);
				observedGrabbable = null;
			}
		}

		/// <summary>
		/// Called when an InteractionHand moves a grabbed object
		/// </summary>
		private void HandleObjectMoved(MovementInfo movementInfo) {
			var deltaTime = Time.time - startTime;

			if (deltaTime < 1f)
				return;

			timeData.Add(deltaTime);
            speedData.Add(movementInfo.Delta.magnitude / deltaTime);
		}

	}

}