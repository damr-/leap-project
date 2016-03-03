using System;
using UniRx;

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
		this.value = initialValue;
		this.observable = new Subject<T>();
	}

	public static implicit operator T(ObservableProperty<T> property) {
		return property.Value;
	}

	public IDisposable Subscribe(IObserver<T> observer) {
		return observable.Subscribe(observer);
	}
}