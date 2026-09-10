using Godot;
using System;

/// <summary>
/// Script principal da cena. Apenas imprime mensagem de boas-vindas.
/// O gerenciamento das curvas é feito pelo CurveManager.
/// </summary>
public partial class Main : Node3D
{
	public override void _Ready()
	{
		GD.Print("=== Cena principal carregada! ===");
		GD.Print("Use TAB para alternar entre curvas (Linear / Catmull-Rom / Bézier).");
		GD.Print("Use R para regenerar a curva ativa.");
	}
}
