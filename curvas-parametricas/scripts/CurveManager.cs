using Godot;
using System;

/// <summary>
/// Gerencia a troca entre os modos de curva (Linear, Catmull-Rom, Bézier).
/// Tecla TAB: alterna entre as curvas.
/// Tecla R: regenera a curva atual.
/// </summary>
public partial class CurveManager : Node3D
{
	[Export] public Curve3D LinearCurveNode;
	[Export] public Curve3D CatmullRomCurveNode;
	[Export] public Curve3D BezierCurveNode;
	[Export] public Pointer PointerNode;

	private Curve3D[] curves;
	private string[] curveNames = { "Linear", "Catmull-Rom", "Bézier" };
	private int currentIndex = 0;

	public override void _Ready()
	{
		curves = new Curve3D[] { LinearCurveNode, CatmullRomCurveNode, BezierCurveNode };

		foreach (var c in curves)
		{
			if (c == null)
			{
				GD.PrintErr("CurveManager: Um ou mais nós de curva não foram atribuídos!");
				return;
			}
		}

		ShowCurve(0);
		GD.Print("CurveManager pronto. TAB = trocar curva | R = regenerar.");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.Tab)
			{
				currentIndex = (currentIndex + 1) % curves.Length;
				ShowCurve(currentIndex);
			}
			else if (keyEvent.Keycode == Key.R)
			{
				curves[currentIndex].GenerateCurvePoints(curves[currentIndex].CurveResolution);

				if (PointerNode != null)
				{
					PointerNode.CurveNodePath = PointerNode.GetPathTo(curves[currentIndex]);
					PointerNode.CallDeferred(nameof(Pointer.SetupPath));
				}

				GD.Print($"Curva regenerada: {curveNames[currentIndex]}");
			}
		}
	}

	private void ShowCurve(int index)
	{
		for (int i = 0; i < curves.Length; i++)
		{
			if (curves[i] != null) {
				curves[i].Visible = (i == index);
			}
		}

		if (PointerNode != null && curves[index] != null)
		{
			PointerNode.CurveNodePath = PointerNode.GetPathTo(curves[index]);
			PointerNode.CallDeferred(nameof(Pointer.SetupPath));
		}

		GD.Print($"Curva ativa: {curveNames[index]}");
	}
}
