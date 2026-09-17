using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Personagem que percorre a curva ativa ponto a ponto.
/// </summary>
public partial class Pointer : Node3D
{
	[Export] public NodePath CurveNodePath;
	[Export] public float Speed = 5f;

	private List<Node3D> CurvePoints = new();
	private int CurrentIndex = 0;

	public override void _Ready()
	{
		CallDeferred(nameof(SetupPath));
	}

	public void SetupPath()
	{
		CurvePoints.Clear();
		CurrentIndex = 0;

		var curveNode = GetNodeOrNull<Node3D>(CurveNodePath);
		if (curveNode == null)
		{
			GD.PrintErr("Pointer: Nó da curva não encontrado!");
			return;
		}

		foreach (Node child in curveNode.GetChildren())
		{
			if (child is Node3D point)
			{
				CurvePoints.Add(point);
			}
		}

		GD.Print($"Pointer: Total de pontos na curva ativa = {CurvePoints.Count}");

		if (CurvePoints.Count > 0)
		{
			GlobalPosition = CurvePoints[0].GlobalPosition;

			if (CurvePoints.Count > 1)
			{
				Vector3 lookTarget = CurvePoints[1].GlobalPosition;
				if (GlobalPosition.DistanceTo(lookTarget) > 0.001f)
					LookAt(lookTarget, Vector3.Up);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (CurvePoints.Count < 2 || CurrentIndex >= CurvePoints.Count - 1)
			return;

		Vector3 from = GlobalPosition;
		Vector3 to = CurvePoints[CurrentIndex + 1].GlobalPosition;

		Vector3 toVector = to - from;
		float distanceToNext = toVector.Length();

		if (distanceToNext < 0.01f)
		{
			CurrentIndex++;
			return;
		}

		Vector3 direction = toVector.Normalized();
		float step = Speed * (float)delta;

		if (step >= distanceToNext)
		{
			GlobalPosition = to;
			CurrentIndex++;

			if (CurrentIndex < CurvePoints.Count - 1)
			{
				Vector3 nextTarget = CurvePoints[CurrentIndex + 1].GlobalPosition;
				if (GlobalPosition.DistanceTo(nextTarget) > 0.001f)
				{
					LookAt(nextTarget, Vector3.Up);
				}
			}
		}
		else
		{
			GlobalPosition += direction * step;
			LookAt(to, Vector3.Up);
		}
	}
}
