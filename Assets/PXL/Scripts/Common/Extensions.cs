using System.Text;
using UnityEngine;

public static class Extensions {

	public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax) {
		return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
	}

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

}