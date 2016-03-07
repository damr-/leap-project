using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace PXL.Utility {

	public class AddComponentsToTargets : MonoBehaviour {
		/// <summary>
		/// All the GameObjects which the components will be added to
		/// </summary>
		public List<GameObject> targets;

		/// <summary>
		/// ALl the components added to each one of the <see cref="targets"/>
		/// </summary>
		public List<string> components;

		private string assemblyName;

		private void Awake() {
			assemblyName = Assembly.GetExecutingAssembly().FullName;
			targets.ForEach(AddComponents);
		}

		private void AddComponents(GameObject target) {
			components.ForEach(c => CreateAndAddComponent(target, c));
		}

		private void CreateAndAddComponent(GameObject target, string component) {
			System.Type componentType = Types.GetType(component, assemblyName);
			target.AddComponent(componentType);
		}
	}

}