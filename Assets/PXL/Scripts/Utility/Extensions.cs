using System.Collections.Generic;
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
			var index = Random.Range(minIndex, items.Length);
			return items.ElementAt(index);
		}

		/// <summary>
		/// Tries to retrieve the value for the given key (a Component) from the given dictionary.
		/// If there is no value for the given key, an entry is added.
		/// </summary>
		/// <param name="dictionary">The dictionary to look for the component</param>
		/// <param name="key">The Component as key for the dictionary</param>
		/// <returns>The Component </returns>
		public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) where TV : class where TK : Component {
			TV value;
			if (!dictionary.TryGetValue(key, out value)) {
				value = key.GetComponent<TV>();
				if (value == null)
					throw new MissingReferenceException("GetOrAdd couldn't get the Component of the object!");
				dictionary.Add(key, value);
			}

			return value;
		}

		/// <summary>
		/// Makes sure the given object is not null. If it is, a MissingReferenceException will be thrown with the given message.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="message"></param>
		public static void AssertNotNull(this Object o, string message = "Missing reference, object is null!") {
			if (o == null)
				throw new MissingReferenceException(message);
		}

		/// <summary>
		/// Compares two Vectors for near equality, optionally with the given epsilon.
		/// </summary>
		public static bool Equal(this Vector3 a, Vector3 b, float epsilon = 0.0001f) {
			return Vector3.Magnitude(a - b) < epsilon;
		}

		/// <summary>
		/// Returns whether the given hand and it's leap hand are valid and visible
		/// </summary>
		public static bool IsHandValid(this HandModel hand) {
			return (hand != null && hand.isActiveAndEnabled && hand.GetLeapHand() != null);
		}
	}

}