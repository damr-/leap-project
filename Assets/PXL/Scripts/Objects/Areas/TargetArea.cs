﻿using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Gamemodes;
using PXL.Utility;
using UniRx;
using UnityEngine;

namespace PXL.Objects.Areas {

	[RequireComponent(typeof(Collider))]
	public abstract class TargetArea : MonoBehaviour {

		/// <summary>
		/// What tag an object has to have to be recognised
		/// </summary>
		public Tags.TagType TargetTagType;

		/// <summary>
		/// The actual tag as string
		/// </summary>
		protected string TargetTag;

		/// <summary>
		/// The Collider of this area
		/// </summary>
		protected Collider AreaCollider {
			get {
				return mAreaCollider ?? (mAreaCollider = this.TryGetComponent<Collider>());
			}
		}
		private Collider mAreaCollider;

		/// <summary>
		/// All valid objects that are inside the area
		/// </summary>
		protected List<Collider> Objects = new List<Collider>();

		/// <summary>
		/// Whether this area is active
		/// </summary>
		protected bool AreaActive = true;
		
		protected virtual void Awake() {
			TargetTag = Tags.GetTagString(TargetTagType);
			SetAreaActive(true);
		}

		protected virtual void Update() {
			if (GameMode.GameOver || !AreaActive)
				return;

			Objects = Objects.Purge();
		}

		/// <summary>
		/// Called when a Collider enters the Trigger
		/// </summary>
		protected virtual void OnTriggerEnter(Collider other) {
			HandleTriggerEntered(other);
		}

		/// <summary>
		/// Called when a Collider exits the Trigger
		/// </summary>
		protected virtual void OnTriggerExit(Collider other) {
			HandleTriggerExit(other);
		}

		/// <summary>
		/// Called when an object exits the trigger
		/// </summary>
		protected virtual void HandleTriggerExit(Collider other) {
			Objects.Remove(other);
		}

		/// <summary>
		/// Called when any object enters the trigger
		/// </summary>
		protected virtual void HandleTriggerEntered(Collider other) {
			if (!HasCorrectTag(other.gameObject) || Objects.Contains(other))
				return;

			Objects.Add(other);
			HandleValidOther(other);
		}

		/// <summary>
		/// Called if the other object, which entered the trigger, has the correct tag
		/// </summary>
		protected abstract void HandleValidOther(Collider other);

		/// <summary>
		/// Returns whether the other object has the correct tag
		/// </summary>
		protected bool HasCorrectTag(GameObject other) {
			return other.CompareTag(TargetTag);
		}

		/// <summary>
		/// Enables the trigger and sets the flag
		/// </summary>
		/// <param name="active">Whether this area is active</param>
		protected virtual void SetAreaActive(bool active) {
			AreaCollider.enabled = active;
			AreaActive = active;
		}
	}

}