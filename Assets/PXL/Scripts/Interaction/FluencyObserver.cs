using System;
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

	public class FluencyObserver : InteractionHandSubscriber {
	
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
		private const float TrackFrequency = 30f;

		/// <summary>
		/// How much data is required to invoke the Observable.
		/// </summary>
		private const int RequiredDataAmount = 10;

		/// <summary>
		/// The last time the object's data was tracked
		/// </summary>
		private float lastTrackTime;

		private IDisposable subscription = Disposable.Empty;

		/// <summary>
		/// Invoked when an object is dropped and the observing is finished
		/// </summary>
		public IObservable<TrackData> FinishedObserving { get { return finishedObservingSubject; } }
		private readonly ISubject<TrackData> finishedObservingSubject = new Subject<TrackData>();
		
		private void Update() {
			//if (observedGrabbable == null)
			//	return;

			//if (Time.time - lastTrackTime < 1/TrackFrequency)
			//	return;
			
			//var observedTime = Time.time - startTime;
			//timeData.Add(observedTime);

			//var deltaTime = Time.time - lastTrackTime;
			//var deltaPos = observedGrabbable.transform.position - lastPosition;
			//speedData.Add(deltaPos.magnitude / deltaTime);

			//lastPosition = observedGrabbable.transform.position;
			//lastTrackTime = Time.time;
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			observedGrabbable = grabbable;
			speedData = new List<float>();
			timeData = new List<float>();
			lastPosition = observedGrabbable.transform.position;
			startTime = Time.time;
			lastTrackTime = 0f;

			subscription = Observable.Interval(TimeSpan.FromSeconds(1 / TrackFrequency)).Subscribe(_ => {
				if (observedGrabbable == null) {
					subscription.Dispose();
					return;
				}

				var observedTime = Time.time - startTime;
				timeData.Add(observedTime);

				var deltaTime = Time.time - lastTrackTime;
				var deltaPos = observedGrabbable.transform.position - lastPosition;
				speedData.Add(deltaPos.magnitude / deltaTime);

				lastPosition = observedGrabbable.transform.position;
			});

		}

		protected override void HandleDropped(Grabbable grabbable) {
			subscription.Dispose();

			if (timeData.Count < RequiredDataAmount)
				return;

			var trackData = new TrackData(observedGrabbable, timeData, speedData);
			finishedObservingSubject.OnNext(trackData);
			observedGrabbable = null;
		}

		protected override void HandleMoved(MovementInfo movementInfo) {
			//movement is tracked every frame, not by observable
		}
	}

}