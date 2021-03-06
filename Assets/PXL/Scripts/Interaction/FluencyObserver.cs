﻿using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace PXL.Interaction {

	/// <summary>
	/// Class storing the data of a tracked movement.
	/// The data consists of the <see cref="Grabbable"/> component of the object, a list of time and speed data with the same length
	/// </summary>
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

	/// <summary>
	/// This class reacts to the given hands picking up an object and records the object's movement with a certain interval.
	/// Once dropped, the <see cref="FinishedObserving"/> is invoked for other objects to react, if there is enough data to use.
	/// </summary>
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

		/// <summary>
		/// Disposable for tracking the object 
		/// </summary>
		private IDisposable trackUpdateDisposable = Disposable.Empty;

		/// <summary>
		/// Invoked when an object is dropped and the observing is finished
		/// </summary>
		public IObservable<TrackData> FinishedObserving { get { return finishedObservingSubject; } }
		private readonly ISubject<TrackData> finishedObservingSubject = new Subject<TrackData>();
		
		protected override void HandleGrabbed(Grabbable grabbable) {
			observedGrabbable = grabbable;
			speedData = new List<float>();
			timeData = new List<float>();
			lastPosition = observedGrabbable.transform.position;
			startTime = Time.time;
			lastTrackTime = Time.time;

			trackUpdateDisposable = Observable.Interval(TimeSpan.FromSeconds(1 / TrackFrequency)).Subscribe(_ => {
				if (observedGrabbable == null) {
					trackUpdateDisposable.Dispose();
					return;
				}

				var observedTime = Time.time - startTime;
				var deltaTime = Time.time - lastTrackTime;
				var deltaPos = observedGrabbable.transform.position - lastPosition;

				timeData.Add(observedTime);
				speedData.Add(deltaPos.magnitude / deltaTime);

				lastTrackTime = Time.time;
				lastPosition = observedGrabbable.transform.position;
			});

		}

		protected override void HandleDropped(Grabbable grabbable) {
			trackUpdateDisposable.Dispose();

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