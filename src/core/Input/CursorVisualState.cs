// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Input.Mouse.VisualState;

public enum CursorVisualState {
    /// <summary>
    /// Based Arrow
    /// </summary>
    Normal,

    /// <summary>
    /// Text Input (I-beam)
    /// </summary>
    Input,

    /// <summary>
    /// For Links, Buttons, Interactive UI 
    /// </summary>
    Hand,

    /// <summary>
    /// Precise positioning
    /// </summary>
    Crosshair, 

    /// <summary>
    /// Load indicator
    /// </summary>
    Forbidden,

    /// <summary>
    /// Prohibited action
    /// </summary>
    Wait, 
    
    /// <summary>
    /// Arrow with hourglass
    /// </summary>
    AppStarting, 
    
    /// <summary>
    /// Arrow with question mark
    /// </summary>
    Help,
    
    /// <summary>
    /// Vertical up arrow
    /// </summary>
    UpArrow,
    
    /// <summary>
    /// Empty cursor (left for compatibility with Win16)
    /// </summary>
    Icon,
    
    /// <summary>
    /// Changing size in horizontal 
    /// </summary>
    ResizeWE,
    
    /// <summary>
    /// Changing size in Vertical
    /// </summary>
    ResizeNS, 
    
    /// <summary>
    /// Diagonal NorthWest/SouthEast
    /// </summary>
    ResizeNWSE, 
    
    /// <summary>
    /// Diagonal Northeast/SouthWest
    /// </summary>
    ResizeNESW,

    /// <summary>
    /// 
    /// </summary>
    ResizeALL
}