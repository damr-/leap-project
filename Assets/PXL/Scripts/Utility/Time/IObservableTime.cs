using UniRx;

namespace PXL.Utility.Time {

	public interface IObservableTime {

		IObservable<int> CreateTimer(float interval);
	}
}