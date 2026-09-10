using Godot;
using System;
using System.Numerics;
using Vector3 = Godot.Vector3;
using Vector4 = System.Numerics.Vector4;
using Matrix4x4 = System.Numerics.Matrix4x4;

/// <summary>
/// Curva de Bézier cúbica.
/// Curva de APROXIMAÇÃO, passa
/// apenas pelos extremos de cada segmento de 4 pontos.
/// </summary>
public partial class BezierCurve : Curve3D
{
	private static Matrix4x4 CreateBezierMatrix()
	{
		// Matriz característica da Bézier cúbica
		return new Matrix4x4(
			-1.0f,  3.0f, -3.0f,  1.0f,
			 3.0f, -6.0f,  3.0f,  0.0f,
			-3.0f,  3.0f,  0.0f,  0.0f,
			 1.0f,  0.0f,  0.0f,  0.0f
		);
	}

	public override void GenerateCurvePoints(int curveRes)
	{
		ClearCurvePoints();

		if (ControlPoints.Count < 4)
		{
			GD.PrintErr($"[{Name}] Bézier cúbica requer pelo menos 4 pontos de controle.");
			return;
		}

		GD.Print($"[{Name}] Gerando Bézier cúbica com {ControlPoints.Count} pontos.");

		float step = 1f / curveRes;
		Matrix4x4 bezierMatrix = CreateBezierMatrix();

		// Cada segmento de Bézier usa 4 pontos: P0, P1, P2, P3.
		// Para encadear segmentos, avançamos de 3 em 3 pontos.
		for (int i = 0; i <= ControlPoints.Count - 4; i += 3)
		{
			Vector3 p0 = ControlPoints[i].GlobalPosition;
			Vector3 p1 = ControlPoints[i + 1].GlobalPosition;
			Vector3 p2 = ControlPoints[i + 2].GlobalPosition;
			Vector3 p3 = ControlPoints[i + 3].GlobalPosition;

			Vector4 Gx = new(p0.X, p1.X, p2.X, p3.X);
			Vector4 Gy = new(p0.Y, p1.Y, p2.Y, p3.Y);
			Vector4 Gz = new(p0.Z, p1.Z, p2.Z, p3.Z);

			for (int j = 0; j < curveRes; j++)
			{
				float t = j * step;
				Vector4 T = new(t * t * t, t * t, t, 1f);

				Vector4 basis = MatrixMult(bezierMatrix, T);

				float x = Vector4.Dot(basis, Gx);
				float y = Vector4.Dot(basis, Gy);
				float z = Vector4.Dot(basis, Gz);

				AddCurvePoint(new Vector3(x, y, z));
			}
		}

		// Adiciona o último ponto de controle
		AddCurvePoint(ControlPoints[ControlPoints.Count - 1].GlobalPosition);

		GD.Print($"[{Name}] Pontos da curva Bézier gerados: {CurvePoints.Count}");
	}
}
