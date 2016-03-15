using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PXL.Utility.Time {

	public static class TimeUtils {

		public static IObservable<int> ElapsedIntervals(this IObservableTime time, float intervalSeconds) {
			if (!(intervalSeconds <= 0f)) return time.CreateTimer(intervalSeconds);

			ISubject<int> subject = new ReplaySubject<int>();
			subject.OnNext(1);
			return subject;
		}

		public static IObservable<Unit> Once(this IObservableTime time, float intervalSeconds) {
			return time.ElapsedIntervals(intervalSeconds).First().Select(i => Unit.Default);
		}

		public static IObservable<float> ValueChangeOverTime(this IObservableTime time, Func<float, float> valueChangeOverTime,
			float sampleFrequency) {
			var sampleInterval = 1/sampleFrequency;
			return time
				.ElapsedIntervals(sampleInterval)
				.Select(ticks => ticks*sampleInterval)
				.Select(valueChangeOverTime)
				.Select(valueGainPerSecond => valueGainPerSecond*sampleInterval);
		}
	}

	public class ObservableTime : MonoBehaviour, IObservableTime {
		private static IObservableTime _instance;
		private readonly Queue<ObservableTimer> recycledTimers = new Queue<ObservableTimer>();

		public static IObservableTime GetInstance() {
			return _instance ?? (_instance = FindObjectOfType<ObservableTime>());
		}

		public IObservable<int> CreateTimer(float interval) {
			var timer = recycledTimers.Count > 0 ? recycledTimers.Dequeue() : gameObject.AddComponent<ObservableTimer>();

			timer.Interval = interval;
			timer.enabled = true;
			return timer;
		}

		public void RecycleTimer(ObservableTimer timer) {
			timer.enabled = false;
			recycledTimers.Enqueue(timer);
		}
	}

	public class ObservableTimer : MonoBehaviour, IObservable<int> {
		private List<IObserver<int>> observers;
		private float interval;
		private float currentTime;
		private int numberOfTicks;
		private ObservableTime timeManager;

		public float Interval {
			get { return interval; }
			set { interval = value; }
		}

		public IDisposable Subscribe(IObserver<int> observer) {
			observers.Add(observer);
			return new TimerSubscription(observer, observers);
		}

		private void Awake() {
			observers = new List<IObserver<int>>();
			timeManager = GetComponent<ObservableTime>();
		}

		private void OnEnable() {
			currentTime = 0;
			numberOfTicks = 0;
		}

		private void OnDisable() {
			observers.Clear();
		}

		private void Update() {
			currentTime += UnityEngine.Time.deltaTime;

			if (observers.Count == 0) {
				timeManager.RecycleTimer(this);
			}

			if (!(currentTime >= interval))
				return;
			numberOfTicks++;
			foreach (var t in observers) {
				t.OnNext(numberOfTicks);
			}

			currentTime -= interval;
		}
	}

	public class TimerSubscription : IDisposable {
		private readonly IObserver<int> observer;
		private readonly List<IObserver<int>> observers;

		public TimerSubscription(IObserver<int> observer, List<IObserver<int>> observers) {
			this.observer = observer;
			this.observers = observers;
		}

		public void Dispose() {
			observers.Remove(observer);
		}
	}
}