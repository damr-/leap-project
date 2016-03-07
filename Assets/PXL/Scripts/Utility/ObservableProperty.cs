using System;
using UniRx;

namespace PXL.Utility {

	public class ObservableProperty<T> : IObservable<T> {
		private T value;
		private Subject<T> observable;

		public T Value {
			get { return value; }
			set {
				this.value = value;
				observable.OnNext(this.value);
			}
		}

		public ObservableProperty(T initialValue = default(T)) {
			value = initialValue;
			observable = new Subject<T>();
		}

		public static implicit operator T(ObservableProperty<T> property) {
			return property.Value;
		}

		public IDisposable Subscribe(IObserver<T> observer) {
			return observable.Subscribe(observer);
		}
	}

}