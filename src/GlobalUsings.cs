// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
global using OpenTK.Windowing.GraphicsLibraryFramework;
global using OpenTK.Mathematics;
global using System.Drawing;
global using OpenTK.Graphics.OpenGL4;
global using System.Runtime.Versioning;
global using System.Drawing.Imaging;
global using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
global using OpenTK.Windowing.Desktop;
global using OpenTK.Windowing.Common;
global using System.Globalization;

global using Unminal.Script.Core;
global using Unminal.Core.ObjLoader;
global using Unminal.Core.Commands.Structure;
global using Unminal.Core.State;
global using Unminal.Core.PlayerCamera;
global using Unminal.Core.Commands.CommandParser;
global using Unminal.Core.Commands.ExecutedMethods;
global using Unminal.Core.Commands.Executor;
global using Unminal.Core.Commands.Processor;
global using Unminal.Core.Commands.Tokenizer;
global using Console = Unminal.Core.EngineConsole.Console;
global using Unminal.Render.Texture;
global using Unminal.Render.Light;
global using Unminal.Render.MeshProgram;
global using Unminal.Render.ShaderProgram;
global using Unminal.Render.Objects;
global using Unminal.Render.Primitive._2D;
global using Unminal.Render.SkyBox;
global using Unminal.Render.Billboards;
global using Unminal.UI.InputFieldRender;
global using Unminal.UI.ButtonRender;
global using Unminal.UI.ButtonEngine;
global using Unminal.UI.ButtonObject;
global using Unminal.UI.TextRender.FontAtlas;
global using Unminal.UI.TextRender.Glyph;
global using Unminal.UI.TextRender.TextRenderer;
global using Unminal.UI.EventBus;
global using Unminal.UI.Factory;
global using Unminal.Utils.Colors;
global using Unminal.Utils.GetPath;
global using Unminal.Utils.ConfigManager;
global using Unminal.Utils.Loging;