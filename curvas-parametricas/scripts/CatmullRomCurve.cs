using Godot;
using System;
using System.Numerics;
using System.Collections.Generic;
using Vector3 = Godot.Vector3;
using Vector4 = System.Numerics.Vector4;
using Matrix4x4 = System.Numerics.Matrix4x4;

public partial class CatmullRomCurve : Curve3D
{
	private static Matrix4x4 CreateCatmullRomMatrix()
	{
		return new Matrix4x4(
			-0.5f,  1.5f, -1.5f,  0.5f,
			 1.0f, -2.5f,  2.0f, -0.5f,
			-0.5f,  0.0f,  0.5f,  0.0f,
			 0.0f,  1.0f,  0.0f,  0.0f
		);
	}

	public override void GenerateCurvePoints(int curveResolution)
	{
		ClearCurvePoints();

		if (ControlPoints.Count < 4)
		{
			GD.Print("Catmull-Rom requer pelo menos 4 pontos de controle.");
			return;
		}

		GD.Print("Gerando Catmull-Rom com ", ControlPoints.Count, " pontos.");

		float step = 1f / curveResolution;
		
		Matrix4x4 catmullRomMatrix = CreateCatmullRomMatrix();

		// Cria uma lista estendida com cópia do primeiro e último
		List<Node3D> extendedPoints = new(ControlPoints);
		extendedPoints.Insert(0, ControlPoints[0]); // Primeiro duplicado no início
		extendedPoints.Add(ControlPoints[ControlPoints.Count - 1]);  // Último duplicado no fim

		// Agora iteramos normalmente
		for (int i = 0; i < extendedPoints.Count - 3; i++)
		{
			Vector3 p0 = extendedPoints[i].GlobalPosition;
			Vector3 p1 = extendedPoints[i + 1].GlobalPosition;
			Vector3 p2 = extendedPoints[i + 2].GlobalPosition;
			Vector3 p3 = extendedPoints[i + 3].GlobalPosition;

			Vector4 Gx = new(p0.X, p1.X, p2.X, p3.X);
			Vector4 Gy = new(p0.Y, p1.Y, p2.Y, p3.Y);
			Vector4 Gz = new(p0.Z, p1.Z, p2.Z, p3.Z);

			for (int j = 0; j < curveResolution; j++)
			{
				float t = j * step;
				Vector4 T = new(t * t * t, t * t, t, 1f);
				
				Vector4 basis = MatrixMult(catmullRomMatrix,T); // M · T

				float x = Vector4.Dot(basis, Gx);
				float y = Vector4.Dot(basis, Gy);
				float z = Vector4.Dot(basis, Gz);

				AddCurvePoint(new Vector3(x, y, z));
			}
		}

		GD.Print("Pontos da curva Catmull-Rom gerados.");
	}

}
