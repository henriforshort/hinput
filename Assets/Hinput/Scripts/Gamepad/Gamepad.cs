﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HinputClasses.Internal;

namespace HinputClasses {
	/// <summary>
	/// Hinput class representing a gamepad.
	/// </summary>
	public class Gamepad {
		// --------------------
		// ID
		// --------------------

		/// <summary>
		/// The index of a gamepad in the gamepad list of Hinput. AnyGamepad returns -1.
		/// </summary>
		public int index { get; protected set; }
		
		/// <summary>
		/// The name of a gamepad, like "Gamepad0" or "AnyGamepad".
		/// </summary>
		public string name { get; protected set; }

		/// <summary>
		/// The full name of a gamepad, like "Windows_Gamepad0" or "Mac_AnyGamepad".
		/// </summary>
		public string fullName { get; protected set; }

		/// <summary>
		/// The type of a gamepad, like "Controller (Xbox One For Windows)", "Wireless Controller" or "AnyGamepad".
		/// </summary>
		public virtual string type {
			get {
				if (Input.GetJoystickNames().Length <= index) return "";
				return Input.GetJoystickNames()[index];
			}
		}
		
		/// <summary>
		/// Returns true if a gamepad is currently connected. Returns false otherwise.<br/> <br/>
		/// On AnyGamepad, returns true if at least one gamepad is currently connected. Returns false otherwise.
		/// </summary>
		public virtual bool isConnected { get { return type != ""; } }
		
		
		// --------------------
		// ENABLED
		// --------------------

		private bool enableWhenConnected = true;
		/// <summary>
		/// Returns true if a gamepad is being tracked by Hinput. Returns false otherwise.<br/> <br/>
		/// On AnyGamepad, returns true if AnyGamepad is enabled (this does NOT give any information on regular
		/// gamepads). Returns false otherwise.
		/// </summary>
		public bool isEnabled { get; protected set; }
		
		/// <summary>
		/// Enable a gamepad so that Hinput starts tracking it. <br/> <br/>
		/// This method is called automatically on a gamepad the first time it is connected, except if it was disabled
		/// in Settings. Calling this method on AnyGamepad only enables AnyGamepad.
		/// </summary>
		public void Enable() {
			isEnabled = true;
		}

		/// <summary>
		/// Reset and disable a gamepad so that Hinput stops tracking it. <br/> <br/>
		/// This may improve performance. Calling this method on AnyGamepad only disables AnyGamepad.
		/// </summary>
		public void Disable() {
			buttons.ForEach(button => button.Reset());
			sticks.ForEach(stick => stick.Reset());
			anyInput.Reset();
			StopVibration();
			
			isEnabled = false;
		}


		// --------------------
		// CONSTRUCTOR
		// --------------------

		protected Gamepad() { }

		public Gamepad (int index) {
			this.index = index;
			name = "Gamepad" + index;
			fullName = Utils.os + "_" + name;
			isEnabled = false;
			enableWhenConnected = (index < Settings.amountOfGamepads);
			
			leftStick = new Stick ("LeftStick", this, 0, !Settings.disableLeftStick);
			rightStick = new Stick ("RightStick", this, 1, !Settings.disableRightStick);
			dPad = new Stick ("DPad", this, 2, !Settings.disableDPad);
			vibration = new Vibration (index);
			
			SetUp();
		}
			
		protected void SetUp() {
			A = new Button ("A", this, 0, !Settings.disableA); 
			B = new Button ("B", this, 1, !Settings.disableB);
			X = new Button ("X", this, 2, !Settings.disableX);
			Y = new Button ("Y", this, 3, !Settings.disableY);
			
			leftBumper = new Button ("LeftBumper", this, 4, !Settings.disableLeftBumper);
			rightBumper = new Button ("RightBumper", this, 5, !Settings.disableRightBumper);
			leftTrigger = new Trigger ("LeftTrigger", this, 6, !Settings.disableLeftTrigger);
			rightTrigger = new Trigger ("RightTrigger", this, 7, !Settings.disableRightTrigger);
			
			back = new Button ("Back", this, 8, !Settings.disableBack);
			start = new Button ("Start", this, 9, !Settings.disableStart);
			leftStickClick = new Button ("LeftStickClick", this, 10, !Settings.disableLeftStickClick);
			rightStickClick = new Button ("RightStickClick", this, 11, !Settings.disableRightStickClick);
			xBoxButton = new Button ("XBoxButton", this, 12, !Settings.disableXBoxButton);

			anyInput = new AnyInput("AnyInput", this, !Settings.disableAnyInput);
			
			sticks = new List<Stick> { leftStick, rightStick, dPad };
			buttons = new List<Pressable> {
				A, B, X, Y,
				leftBumper, rightBumper, leftTrigger, rightTrigger,
				back, start, leftStickClick, rightStickClick, xBoxButton
			};
		}

		
		// --------------------
		// UPDATE
		// --------------------

		/// <summary>
		/// Hinput internal method. You don't need to use it.
		/// </summary>
		public void Update () {
			if (UpdateIsRequired() == false) return;

			buttons.ForEach(button => button.Update());
			sticks.ForEach(stick => stick.Update());
			anyInput.Update();
			vibration.Update();
		}

		protected virtual bool UpdateIsRequired() {
			if (!isEnabled && enableWhenConnected && isConnected) {
				Enable();
				enableWhenConnected = false;
			}

			return isEnabled;
		}

		
		// --------------------
		// PUBLIC PROPERTIES
		// --------------------

		/// <summary>
		/// The A button of a gamepad.
		/// </summary>
		public Button A { get; private set; }

		/// <summary>
		/// The B button of a gamepad.
		/// </summary>
		public Button B { get; private set; }

		/// <summary>
		/// The X button of a gamepad.
		/// </summary>
		public Button X { get; private set; }

		/// <summary>
		/// The Y button of a gamepad.
		/// </summary>
		public Button Y { get; private set; }

		/// <summary>
		/// The left bumper of a gamepad.
		/// </summary>
		public Button leftBumper { get; private set; }

		/// <summary>
		/// The right bumper of a gamepad.
		/// </summary>
		public Button rightBumper { get; private set; }

		/// <summary>
		/// The back button of a gamepad.
		/// </summary>
		public Button back { get; private set; }

		/// <summary>
		/// The start button of a gamepad.
		/// </summary>
		public Button start { get; private set; }

		/// <summary>
		/// The left stick click of a gamepad.
		/// </summary>
		public Button leftStickClick { get; private set; }

		/// <summary>
		/// The right stick click of a gamepad.
		/// </summary>
		public Button rightStickClick { get; private set; }

		/// <summary>
		/// The XBox button of a gamepad.<br/> <br/>
		/// Windows and Linux drivers can’t detect the value of this button. Therefore it will be considered released
		/// at all times on these operating systems.
		/// </summary>
		public Button xBoxButton { get; private set; }

		/// <summary>
		/// The left trigger of a gamepad.
		/// </summary>
		public Trigger leftTrigger { get; private set; }
		
		/// <summary>
		/// The right trigger of a gamepad.
		/// </summary>
		public Trigger rightTrigger { get; private set; }

		/// <summary>
		/// The left stick of a gamepad.
		/// </summary>
		public Stick leftStick { get; protected set; }

		/// <summary>
		/// The right stick of a gamepad.
		/// </summary>
		public Stick rightStick { get; protected set; }
		
		/// <summary>
		/// The D-pad of a gamepad.
		/// </summary>
		public Stick dPad { get; protected set; }

		/// <summary>
		/// The virtual button that returns every input of a gamepad at once.<br/> <br/>
		/// The position of AnyInput is the highest position for all inputs on that gamepad. AnyInput is considered
		/// pressed if any input on that gamepad is pressed.
		/// </summary>
		public AnyInput anyInput { get; private set; }

		/// <summary>
		/// The list containing the sticks of a gamepad, in the following order : { leftStick, rightStick, dPad }
		/// </summary>
		public List<Stick> sticks { get; private set; }

		/// <summary>
		/// The list containing the buttons of a gamepad, in the following order : { A, B, X, Y, left bumper, right
		/// bumper, left trigger, right trigger, back, start, left stick click, right stick click, XBox button }
		/// </summary>
		public List<Pressable> buttons { get; private set; }

		/// <summary>
		/// The list of all inputs that are currently being pressed on a gamepad.
		/// </summary>
		public List<Pressable> activeInputs {
			get {
				List<Pressable> allButtons = new List<Pressable>();
				allButtons.AddRange(buttons);
				allButtons.AddRange(sticks.Select(stick => (Pressable)stick.inPressedZone));
				return allButtons.Where(input => input).ToList();
			}
		}

		// --------------------
		// VIBRATION
		// --------------------
		
		protected Vibration vibration;

		/// <summary>
		/// The intensity at which the left motor of a gamepad is currently vibrating.<br/> <br/>
		/// On AnyGamepad, returns -1.
		/// </summary>
		public virtual float leftVibration { get { return vibration.currentLeft; } }

		/// <summary>
		/// The intensity at which the right motor of a gamepad is currently vibrating.<br/> <br/>
		/// On AnyGamepad, returns -1.
		/// </summary>
		public virtual float rightVibration { get { return vibration.currentRight; } }
		
		/// <summary>
		/// Vibrate a gamepad.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate() {
			vibration.Vibrate(
				Settings.vibrationDefaultLeftIntensity, 
				Settings.vibrationDefaultRightIntensity, 
				Settings.vibrationDefaultDuration);
		}

		/// <summary>
		/// Vibrate a gamepad for duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float duration) {
			vibration.Vibrate(
				Settings.vibrationDefaultLeftIntensity, 
				Settings.vibrationDefaultRightIntensity, 
				duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor, and an intensity of rightIntensity
		/// on the right motor.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float leftIntensity, float rightIntensity) {
			vibration.Vibrate(
				leftIntensity, 
				rightIntensity, 
				Settings.vibrationDefaultDuration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor and an intensity of rightIntensity
		/// on the right motor, for duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(float leftIntensity, float rightIntensity, float duration) {
			vibration.Vibrate(
				leftIntensity, 
				rightIntensity, 
				duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an intensity over time based on an animation curve.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(AnimationCurve curve) {
			vibration.Vibrate(curve, curve);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an intensity over time based on two animation curves, one for the left side and one
		/// for the right side.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(AnimationCurve leftCurve, AnimationCurve rightCurve) {
			vibration.Vibrate(leftCurve, rightCurve);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset) {
			vibration.Vibrate(vibrationPreset, 1, 1, 1);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The duration of the preset is multiplied by duration.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float duration) {
			vibration.Vibrate(vibrationPreset, 1, 1, duration);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The left intensity of the preset is multiplied by leftIntensity, and its right intensity is multiplied by
		/// rightIntensity.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity) {
			vibration.Vibrate(vibrationPreset, leftIntensity, rightIntensity, 1);
		}

		/// <summary>
		/// Vibrate a gamepad with an intensity and a duration based on a vibration preset.
		/// The left intensity of the preset is multiplied by leftIntensity, its right intensity is multiplied by
		/// rightIntensity, and its duration is multiplied by duration.<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void Vibrate(VibrationPreset vibrationPreset, float leftIntensity, float rightIntensity, 
			float duration) {
			vibration.Vibrate(vibrationPreset, leftIntensity, rightIntensity, duration);
		}
		
		/// <summary>
		/// Vibrate a gamepad with an instensity of leftIntensity on the left motor, and an intensity of rightIntensity
		/// on the right motor, FOREVER. Don't forget to call StopVibration!<br/> <br/>
		/// Calling this on AnyGamepad vibrates all gamepads.
		/// </summary>
		public virtual void VibrateAdvanced(float leftIntensity, float rightIntensity) {
			vibration.VibrateAdvanced(leftIntensity, rightIntensity);
		}

		/// <summary>
		/// Stop all vibrations on a gamepad immediately.<br/> <br/>
		/// Calling this on AnyGamepad stops all gamepads.
		/// </summary>
		public virtual void StopVibration () {
			vibration.StopVibration(0);
		}

		/// <summary>
		/// Stop all vibrations on a gamepad progressively over duration seconds.<br/> <br/>
		/// Calling this on AnyGamepad stops all gamepads.
		/// </summary>
		public virtual void StopVibration (float duration) {
			vibration.StopVibration(duration);
		}
	}
}