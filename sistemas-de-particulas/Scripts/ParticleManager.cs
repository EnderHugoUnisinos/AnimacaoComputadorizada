using Godot;
using System;

public partial class ParticleManager : Node
{
    private GpuParticles3D Particles;
    private Label UILabel;

    private Mesh QuadMesh;
    private Mesh SphereMesh;
    private Mesh BoxMesh;

    private StandardMaterial3D ParticleMaterial;

    private int CurrentMode = 0;
    private const int ModeCount = 6;

    // Colors (could be a list/dictionary, but I'll keep it separate for clarity)
    private readonly Color Orange = new Color(1.00f, 0.45f, 0.10f);
    private readonly Color Blue   = new Color(0.20f, 0.60f, 1.00f);
    private readonly Color Purple = new Color(0.80f, 0.20f, 0.90f);
    private readonly Color Green  = new Color(0.20f, 1.00f, 0.40f);
    private readonly Color Yellow = new Color(1.00f, 0.90f, 0.20f);
    private readonly Color Red    = new Color(1.00f, 0.20f, 0.20f);


    public override void _Ready()
    {
        Particles = GetNode<GpuParticles3D>("../GPUParticles3D");
        UILabel   = GetNode<Label>("../UI/Label");

        ParticleMaterial = CreateParticleMaterial();

        QuadMesh   = CreateMeshWithMaterial(new QuadMesh());
        SphereMesh = CreateMeshWithMaterial(new SphereMesh());
        BoxMesh    = CreateMeshWithMaterial(new BoxMesh());

        Particles.Amount = 500;
        Particles.Lifetime = 3.0f;
        Particles.Explosiveness = 0.0f;
        Particles.OneShot = false;
        Particles.LocalCoords  = false;

        Particles.ProcessMaterial = new ParticleProcessMaterial();

        ApplyMode(0);
        UpdateUI();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
            return;

        // 1-6: change modes
        if (key.Keycode >= Key.Key1 && key.Keycode <= Key.Key6)
        {
            int mode = (int)key.Keycode - (int)Key.Key1;
            if (mode < ModeCount)
            {
                CurrentMode = mode;
                ApplyMode(mode);
                UpdateUI();
            }
        }

        else if (key.Keycode == Key.R)
        {
            Particles.Restart();
        }
    }

    /// <summary>
    /// Creates de particle's material.
    /// </summary>
    private StandardMaterial3D CreateParticleMaterial()
    {
        var mat = new StandardMaterial3D
        {
            VertexColorUseAsAlbedo = true,

            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,

			//I'll keep it unshaded
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,

            CullMode = BaseMaterial3D.CullModeEnum.Disabled,

            AlbedoColor = Colors.White,
        };
        return mat;
    }

    private Mesh CreateMeshWithMaterial(Mesh mesh)
    {
        mesh.SurfaceSetMaterial(0, ParticleMaterial);
        return mesh;
    }

    /// <summary>
    /// Creates a CurveTexture to represent size along particle lifetime.
    /// </summary>
    private CurveTexture MakeScaleCurve(float start, float end, bool shrinkAtEnd = false)
    {
        var curve = new Curve();
        curve.AddPoint(new Vector2(0.0f, start));
        curve.AddPoint(new Vector2(0.5f, 1.0f));
        curve.AddPoint(new Vector2(1.0f, shrinkAtEnd ? 0.0f : end));
        return new CurveTexture { Curve = curve };
    }

    private void ApplyMode(int mode)
    {
        ResetParameters();

        switch (mode)
        {
            case 0: ConfigureMode0(); break;
            case 1: ConfigureMode1(); break;
            case 2: ConfigureMode2(); break;
            case 3: ConfigureMode3(); break;
            case 4: ConfigureMode4(); break;
            case 5: ConfigureMode5(); break;
        }

        Particles.Restart();
    }

    private void ResetParameters()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

		//Sets emmission direction, scale, velocity and turbulence

        pm.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Point;
        pm.EmissionSphereRadius = 1.0f;
        pm.EmissionBoxExtents = new Vector3(1, 1, 1);
        pm.EmissionRingRadius = 1.0f;
        pm.EmissionRingInnerRadius = 0.5f;
        pm.EmissionRingHeight = 0.1f;

        pm.Direction = new Vector3(0, -1, 0);
        pm.Spread = 45.0f;
        pm.InitialVelocityMin = 1.0f;
        pm.InitialVelocityMax = 5.0f;
        pm.Gravity = new Vector3(0, -9.8f, 0);
        pm.DampingMin = 0.0f;
        pm.DampingMax = 0.0f;

        pm.ScaleMin = 0.15f;
        pm.ScaleMax = 0.30f;
        pm.ScaleCurve = null;
        pm.Color = Colors.White;

        pm.AngleMin = 0.0f;
        pm.AngleMax = 0.0f;
        pm.AngularVelocityMin = 0.0f;
        pm.AngularVelocityMax = 0.0f;

        pm.TurbulenceEnabled = false;
        pm.TurbulenceNoiseStrength = 0.0f;
        pm.TurbulenceNoiseScale = 1.0f;

        Particles.DrawPass1 = QuadMesh;

        foreach (Node child in Particles.GetChildren())
        {
            if (child is GpuParticlesAttractorSphere3D)
                child.QueueFree();
        }
    }

// configures each of the modes
    private void ConfigureMode0()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape       = ParticleProcessMaterial.EmissionShapeEnum.Point;
        pm.Direction           = new Vector3(0, 1, 0);
        pm.Spread              = 30.0f;
        pm.InitialVelocityMin  = 3.0f;
        pm.InitialVelocityMax  = 6.0f;
        pm.Gravity             = new Vector3(0, -9.8f, 0);

        pm.Color = Orange;
        Particles.Lifetime = 2.5f;
        Particles.DrawPass1 = QuadMesh;
    }

    private void ConfigureMode1()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape        = ParticleProcessMaterial.EmissionShapeEnum.Sphere;
        pm.EmissionSphereRadius = 1.5f;
        pm.Gravity              = new Vector3(0, -4.0f, 0);

        AddAttractorSphere(new Vector3(0, 3, 0), 3.0f, 3.0f);

        pm.Color = Blue;
        Particles.Lifetime = 3.5f;
        Particles.DrawPass1 = SphereMesh;
    }

    private void ConfigureMode2()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape      = ParticleProcessMaterial.EmissionShapeEnum.Box;
        pm.EmissionBoxExtents = new Vector3(2, 0.3f, 2);
        pm.Gravity            = new Vector3(0, -1.0f, 0);

        pm.TurbulenceEnabled       = true;
        pm.TurbulenceNoiseStrength = 2.0f;
        pm.TurbulenceNoiseScale    = 1.5f;

        pm.ScaleMin = 0.10f;
        pm.ScaleMax = 0.35f;
        pm.ScaleCurve = MakeScaleCurve(1.0f, 1.0f, shrinkAtEnd: true); // MORRE encolhendo até 0

        pm.Color = Green;
        Particles.Lifetime = 4.0f;
        Particles.DrawPass1 = QuadMesh;
    }

    private void ConfigureMode3()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape            = ParticleProcessMaterial.EmissionShapeEnum.Ring;
        pm.EmissionRingRadius       = 2.0f;
        pm.EmissionRingInnerRadius  = 1.5f;
        pm.EmissionRingHeight       = 0.2f;

        pm.Direction          = new Vector3(0, 0, 1);
        pm.Spread             = 360.0f;
        pm.InitialVelocityMin = 3.0f;
        pm.InitialVelocityMax = 6.0f;
        pm.Gravity            = new Vector3(0, -9.8f, 0);

        pm.Color = Purple;
        Particles.Lifetime = 3.0f;
        Particles.DrawPass1 = BoxMesh;
    }

    private void ConfigureMode4()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Point;
        pm.Direction     = new Vector3(0, 1, 0);
        pm.Spread        = 180.0f;
        pm.InitialVelocityMin = 10.0f;
        pm.InitialVelocityMax = 20.0f;
        pm.Gravity       = new Vector3(0, -5.0f, 0);
        pm.DampingMin    = 1.0f;
        pm.DampingMax    = 2.0f;

        pm.ScaleMin = 0.10f;
        pm.ScaleMax = 0.20f;
        pm.ScaleCurve = MakeScaleCurve(1.0f, 1.0f, shrinkAtEnd: true); // MORRE encolhendo

        pm.Color = Red;
        Particles.Lifetime = 2.0f;
        Particles.DrawPass1 = QuadMesh;
    }

    private void ConfigureMode5()
    {
        var pm = (ParticleProcessMaterial)Particles.ProcessMaterial;

        pm.EmissionShape        = ParticleProcessMaterial.EmissionShapeEnum.Sphere;
        pm.EmissionSphereRadius = 2.0f;
        pm.Gravity              = new Vector3(0, -1.5f, 0);

        pm.AngularVelocityMin = -180.0f;
        pm.AngularVelocityMax =  180.0f;

        pm.TurbulenceEnabled       = true;
        pm.TurbulenceNoiseStrength = 3.0f;
        pm.TurbulenceNoiseScale    = 1.2f;

        pm.ScaleMin = 0.20f;
        pm.ScaleMax = 0.40f;

        pm.Color = Yellow;
        Particles.Lifetime = 5.0f;
        Particles.DrawPass1 = SphereMesh;
    }

    private void AddAttractorSphere(Vector3 position, float radius, float strength)
    {
        var attractor = new GpuParticlesAttractorSphere3D
        {
            Radius   = radius,
            Strength = strength,
            Position = position,
            Attenuation = 1.0f,
        };
        Particles.AddChild(attractor);
    }

    private void UpdateUI()
    {
        string modeName = CurrentMode switch
        {
            0 => "Modo 0 — Emissor: Ponto  |  Gravidade  |  Morte: Tempo",
            1 => "Modo 1 — Emissor: Esfera |  Gravidade + Atração |  Morte: Tempo",
            2 => "Modo 2 — Emissor: Caixa  |  Turbulência + Escala |  Morte: Encolhe até 0",
            3 => "Modo 3 — Emissor: Anel   |  Vel. Radial + Gravidade |  Morte: Tempo",
            4 => "Modo 4 — Emissor: Ponto  |  Alta Vel. + Damping |  Morte: Encolhe até 0",
            5 => "Modo 5 — Emissor: Esfera |  Rotação + Turbulência |  Morte: Tempo",
            _ => "Modo desconhecido"
        };

        UILabel.Text =
            $"{modeName}\n\n" +
            "Teclas 1-6: trocar de modo\n" +
            "Tecla R: reiniciar partículas";
    }
}