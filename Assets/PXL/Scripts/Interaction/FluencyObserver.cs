using UnityEngine;
using System.Collections.Generic;
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
		/// The last position of the observed Object
		/// </summary>
		private Vector3 lastPosition;

		/// <summary>
		/// How often per second the object's data is tracked
		/// </summary>
		private const float TrackFrequency = 10f;

		/// <summary>
		/// How much data is required to invoke the Observable.
		/// </summary>
		private const int RequiredDataAmount = 10;

		/// <summary>
		/// The last time the object's data was tracked
		/// </summary>
		private float lastTrackTime;

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
			}
		}

		private void Update() {
			if (observedGrabbable == null)
				return;

			if (Time.time - lastTrackTime < 1/TrackFrequency)
				return;
			
			var observedTime = Time.time - startTime;

			timeData.Add(observedTime);

			var deltaTime = Time.time - lastTrackTime;
			var deltaPos = observedGrabbable.transform.position - lastPosition;
            
			speedData.Add(deltaPos.magnitude / deltaTime);

			lastPosition = observedGrabbable.transform.position;
			lastTrackTime = Time.time;
		}

		/// <summary>
		/// Called when an InteractionHand grabs or drops an object
		/// </summary>
		private void HandleGrabStateChanged(Grabbable grabbable, bool grabbed) {
			if (grabbed) {
				observedGrabbable = grabbable;
				speedData = new List<float>();
				timeData = new List<float>();
				lastPosition = observedGrabbable.transform.position;
				startTime = Time.time;
				lastTrackTime = 0f;
			}
			else {
				if (timeData.Count < RequiredDataAmount)
					return;

				var trackData = new TrackData(observedGrabbable, timeData, speedData);
				finishedObservingSubject.OnNext(trackData);
				observedGrabbable = null;
			}
		}

	}

}