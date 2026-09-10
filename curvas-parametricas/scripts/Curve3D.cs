using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using Vector3 = Godot.Vector3;
using Vector4 = System.Numerics.Vector4;
using Matrix4x4 = System.Numerics.Matrix4x4;

public partial class Curve3D : Node3D
{
	[Export] public PackedScene ControlPointScene;
	[Export] public PackedScene CurvePointScene;
	[Export] public Material CurvePointMaterial;

	[Export(PropertyHint.Range, "1,100,1")]
	public int CurveResolution = 10;

	// Array -> List : Para consistencia com o resto dos codigos.
	protected List<Node3D> ControlPoints = new();
	protected List<Node3D> CurvePoints = new();

	public override void _Ready()
	{
		Visible = false;
		CallDeferred(nameof(CollectControlPoints));
	}

	public void CollectControlPoints()
	{
		ControlPoints.Clear();

		var container = GetNodeOrNull<Node3D>("../ControlPoints");

		if (container != null)
		{
			foreach (Node child in container.GetChildren())
			{
				if (child is Node3D point)
				{
					ControlPoints.Add(point);
				}
			}

			GD.Print($"[{Name}] Pontos de controle carregados: {ControlPoints.Count}");
		}
		else
		{
			GD.PrintErr($"[{Name}] Nó 'ControlPoints' não encontrado.");
		}

		if (ControlPoints.Count >= 2)
		{
			GenerateCurvePoints(CurveResolution);
		}
	}

	protected void AddCurvePoint(Vector3 position)
	{
		if (CurvePointScene == null)
		{
			GD.PrintErr($"[{Name}] CurvePointScene não atribuída.");
			return;
		}

		var instance = CurvePointScene.Instantiate<Node3D>();
		instance.Position = position;

		if (CurvePointMaterial != null && instance is MeshInstance3D meshInstance)
		{
			meshInstance.MaterialOverride = CurvePointMaterial;
		}

		AddChild(instance);
		CurvePoints.Add(instance);
	}

	protected void ClearCurvePoints()
	{
		foreach (var point in CurvePoints)
		{
			if (IsInstanceValid(point))
			{
				RemoveChild(point);
				point.QueueFree();
			}
		}

		CurvePoints.Clear();
	}

	public virtual void GenerateCurvePoints(int curveRes)
	{
		GD.Print($"[{Name}] GenerateCurvePoints não implementado.");
	}

	public Vector4 MatrixMult(Matrix4x4 m, Vector4 t)
	{
		return new Vector4(
			Vector4.Dot(new Vector4(m.M11, m.M12, m.M13, m.M14), t),
			Vector4.Dot(new Vector4(m.M21, m.M22, m.M23, m.M24), t),
			Vector4.Dot(new Vector4(m.M31, m.M32, m.M33, m.M34), t),
			Vector4.Dot(new Vector4(m.M41, m.M42, m.M43, m.M44), t)
		);
	}
}
