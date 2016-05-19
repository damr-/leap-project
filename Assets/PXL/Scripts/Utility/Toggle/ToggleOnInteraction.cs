using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Interaction;
using UnityEngine;

namespace PXL.Utility.Toggle {

	public class ToggleOnInteraction : InteractionHandSubscriber {

		[Serializable]
		public class ToggleSetting {
			public HandSide HandSide;
			public InteractionType InteractionType;

			public List<Behaviour> EnableBehaviours = new List<Behaviour>();
			public List<Behaviour> DisableBehaviours = new List<Behaviour>();
			public List<Behaviour> ToggleBehaviours = new List<Behaviour>();

			public List<GameObject> EnableGameObjects = new List<GameObject>();
			public List<GameObject> DisableGameObjects = new List<GameObject>();
			public List<GameObject> ToggleGameObjects = new List<GameObject>();

			/// <summary>
			/// How often the toggle should be executed. -1 for infinite
			/// </summary>
			public int Executions = 1;

			/// <summary>
			/// The current count of executions
			/// </summary>
			private int currentExecutions;

			public void TryExecute() {
				if (currentExecutions >= Executions)
					return;

				EnableBehaviours.ForEach(b => b.enabled = true);
				DisableBehaviours.ForEach(b => b.enabled = false);
				ToggleBehaviours.ForEach(b => b.enabled = !b.enabled);

				EnableGameObjects.ForEach(o => o.SetActive(true));
				DisableGameObjects.ForEach(o => o.SetActive(false));
				ToggleGameObjects.ForEach(o => o.SetActive(!o.activeInHierarchy));

				currentExecutions++;
			}

			public void AssertReferences() {
				EnableBehaviours.ForEach(b => b.AssertNotNull("Missing enable Behaviour reference!"));
				DisableBehaviours.ForEach(b => b.AssertNotNull("Missing disable Behaviour reference!"));
				ToggleBehaviours.ForEach(b => b.AssertNotNull("Missing toggle Behaviour reference!"));
				EnableGameObjects.ForEach(b => b.AssertNotNull("Missing enable GameObject reference!"));
				DisableGameObjects.ForEach(b => b.AssertNotNull("Missing disable GameObject reference!"));
				ToggleGameObjects.ForEach(b => b.AssertNotNull("Missing toggle GameObject reference!"));
			}
		}

		/// <summary>
		/// The ToggleSettings for this object
		/// </summary>
		public List<ToggleSetting> Settings = new List<ToggleSetting>();

		protected override void Start() {
			base.Start();

			Settings.ForEach(s => {
				s.AssertReferences();
				if (s.Executions == 0)
					Debug.LogWarning("A ToggleBehavioursOnInteraction setting has set 0 executions by default. Mistake?");
			});
		}

		protected override void HandleGrabbed(Grabbable grabbable) {
			var validTypeSettings = Settings.Where(s => s.InteractionType == InteractionType.Grab).ToList();

			var validSideSettings = new List<ToggleSetting>();
			foreach (var setting in validTypeSettings) {
				if (setting.HandSide == HandSide.Both) {
					validSideSettings.Add(setting);
					continue;
				}

				var side = GetHandSideIfValid(grabbable);

				if (setting.HandSide == side)
					validSideSettings.Add(setting);
			}

			foreach (var validSideSetting in validSideSettings)
				validSideSetting.TryExecute();
		}

		protected override void HandleDropped(Grabbable grabbable) { }

		protected override void HandleMoved(MovementInfo movementInfo) { }

	}

}