﻿// Author : Henri Couvreur for hiloqo, 2019
// Contact : couvreurhenri@gmail.com, hiloqo.games@gmail.com

using System.Collections.Generic;
using System.Linq;
using HinputClasses;
using HinputClasses.Internal;
using UnityEngine;

/// <summary>
/// The main class of the Hinput package, from which you can access gamepads.
/// </summary>
public static class Hinput {
	// --------------------
	// GAMEPAD
	// --------------------

	private static List<Gamepad> _gamepad;
	/// <summary>
	/// A list of 8 gamepads, labelled 0 to 7.
	/// </summary>
	public static List<Gamepad> gamepad { 
		get {
			Updater.CheckInstance();
			if (_gamepad == null) {
				_gamepad = new List<Gamepad>();
				for (int i=0; i<Utils.maxGamepads; i++) _gamepad.Add(new Gamepad(i));
			}

			return _gamepad; 
		} 
	}
	
	
	// --------------------
	// ANYGAMEPAD
	// --------------------

	private static AnyGamepad _anyGamepad;
	/// <summary>
	/// A virtual gamepad that returns the inputs of every gamepad at once.<br/> <br/>
	///
	/// The position of a button on AnyGamepad is the highest position for that button on all gamepads.<br/> 
	/// The position of a stick on AnyGamepad is the average position of pushed sticks of that type on all
	/// gamepads.<br/> 
	/// Vibrating AnyGamepad vibrates all gamepads.<br/> <br/>
	/// 
	/// Examples: <br/>
	/// - If player 1 pushed their A button and player 2 pushed their B button, both the A and the B button of
	/// AnyGamepad will be pressed.<br/>
	/// - If player 1 pushed their left trigger by 0.2 and player 2 pushed theirs by 0.6, the left trigger of
	/// AnyGamepad will have a position of 0.6.<br/>
	/// - If player 1 positioned their right stick at (-0.2, 0.9) and player 2 has theirs at (0, 0), the
	/// position of the right stick of AnyGamepad will be (-0.2, 0.9).<br/>
	/// - If player 1 positioned their right stick at (-0.2, 0.9) and player 2 has theirs at (0.6, 0.3), the
	/// position of the right stick of AnyGamepad will be the average of both positions, (0.2, 0.6).
	/// </summary>
	public static AnyGamepad anyGamepad { 
		get { 
			Updater.CheckInstance();
			if (_anyGamepad == null) _anyGamepad = new AnyGamepad();
			return _anyGamepad; 
		}
	}


	// --------------------
	// ANYINPUT
	// --------------------
	
	/// <summary>
	/// A virtual button that returns every input of every gamepad at once.
	/// </summary>
	public static Pressable anyInput { get { return anyGamepad.anyInput; } }
	
	
	// --------------------
	// ACTIVE GAMEPADS
	// --------------------

	private static List<Gamepad> _activeGamepads;
	private static int _lastActiveGamepadsUpdateFrame = -1;
	/// <summary>
	/// A list of all gamepads on which at least one button is currently being pressed.
	/// </summary>
	public static List<Gamepad> activeGamepads {
		get {
			if (_lastActiveGamepadsUpdateFrame == Time.frameCount) return _activeGamepads;
                
			_activeGamepads = gamepad.Where(g => g.anyInput).ToList();
			_lastActiveGamepadsUpdateFrame = Time.frameCount;
			return _activeGamepads;
		}
	}
}