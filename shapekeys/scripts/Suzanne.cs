using Godot;
using System;
using System.Collections.Generic;

public partial class Suzanne : Node
{

	public MeshInstance3D SuzanneMesh;
	public AnimationPlayer SuzannePlayer;

	public bool AnimationPlaying;

	private int shapekeyIndex;
	public int ShapekeyIndex
	{
		get => shapekeyIndex;
		set
		{
			if (value >= SuzanneMesh.GetBlendShapeCount())
				value = 0;
			else if (value < 0)
				value = SuzanneMesh.GetBlendShapeCount() - 1;

			shapekeyIndex = value;
		}
	}
	public List<string> ShapekeyNames = ["Happy", "Sad", "Surprised"];

	public Label ValueDisplay;

	public List<Label> labels = new List<Label>();
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SuzanneMesh = GetNode<MeshInstance3D>("Suzanne");
		SuzannePlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		ValueDisplay = GetNode<Label>("CanvasLayer/Value");

		labels.Add(GetNode<Label>("CanvasLayer/Label0"));
		labels.Add(GetNode<Label>("CanvasLayer/Label1"));
		labels.Add(GetNode<Label>("CanvasLayer/Label2"));

		UpdateLabels();	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		
		//play/pause animation
		if (@event.IsActionReleased("ui_accept"))
		{
			for (int i = 0; i < SuzanneMesh.GetBlendShapeCount(); i++) { SuzanneMesh.SetBlendShapeValue(i, 0.0f); }
			AnimationPlaying = !AnimationPlaying;
			if (AnimationPlaying) SuzannePlayer.Play("Shape Keys");
			else SuzannePlayer.Stop();
		}

		if(!SuzannePlayer.IsPlaying()) AnimationPlaying = false;
		if (AnimationPlaying) return;


		//select previous
		if (@event.IsActionPressed("ui_left"))
		{
			for (int i = 0; i < SuzanneMesh.GetBlendShapeCount(); i++) { SuzanneMesh.SetBlendShapeValue(i, 0.0f); }
			ShapekeyIndex--;
		}

		//select next
		if (@event.IsActionPressed("ui_right"))
		{
			for (int i = 0; i < SuzanneMesh.GetBlendShapeCount(); i++) { SuzanneMesh.SetBlendShapeValue(i, 0.0f); }
			ShapekeyIndex++;
		}

		float currentShapeValue = SuzanneMesh.GetBlendShapeValue(ShapekeyIndex);
		
		//value up
		if (@event.IsAction("ui_up"))
		{
			currentShapeValue += 0.1f;
		}

		//value down
		if (@event.IsAction("ui_down"))
		{
			currentShapeValue -= 0.1f;
		}

		currentShapeValue = Math.Clamp(currentShapeValue, -1.0f, 1.0f);
		SuzanneMesh.SetBlendShapeValue(ShapekeyIndex, currentShapeValue);

		UpdateLabels();
	}

	public void UpdateLabels()
	{
		foreach (var l in labels)
		{
			l.Scale = new Vector2(1.0f, 1.0f);
		}
		labels[ShapekeyIndex].Scale = new Vector2(1.5f, 1.5f);

		ValueDisplay.Text = SuzanneMesh.GetBlendShapeValue(ShapekeyIndex).ToString();
	}

}
