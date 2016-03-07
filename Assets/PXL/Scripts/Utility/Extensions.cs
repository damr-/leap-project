using System.Linq;
using System.Text;
using UnityEngine;

namespace PXL.Utility {

	public static class Extensions {

		/// <summary>
		/// Maps the value from the given input range to the desired output range
		/// </summary>
		public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax) {
			return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
		}

		/// <summary>
		/// Returns the Component or throws an exception if it is missing
		/// </summary>
		public static T TryGetComponent<T>(this Component component) where T : Component {
			var tryComponent = component.GetComponent<T>();
			if (tryComponent == null) {
				var errorMessage = new StringBuilder();
				errorMessage.Append("Component ");
				errorMessage.Append(component.ToString());
				errorMessage.Append(" is trying to access Component ");
				errorMessage.Append(typeof(T).FullName);
				errorMessage.Append(", but it is missing");
				throw new MissingComponentException(errorMessage.ToString());
			}

			return tryComponent;
		}
		
		/// <summary>
		/// Returns a random item from the given array with optional minimum index.
		/// Maximum index is always the last element in the array.
		/// </summary>
		/// <param name="items">The array with all items</param>
		/// <param name="minIndex">The minimum possible index</param>
		public static T GetRandomElement<T>(this T[] items, int minIndex = 0) where T : struct {
			int index = Random.Range(minIndex, items.Length);
			return items.ElementAt(index);
		}
	}

}