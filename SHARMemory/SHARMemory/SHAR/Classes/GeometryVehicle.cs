using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Drawing;
using static SHARMemory.SHAR.Globals;

namespace SHARMemory.SHAR.Classes;

public class GeometryVehicle : Class
{
    public GeometryVehicle(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public Vehicle Vehicle => Memory.ClassFactory.Create<Vehicle>(ReadUInt32(0));

    public tCompositeDrawable CompositeDrawable => Memory.ClassFactory.Create<tCompositeDrawable>(ReadUInt32(4));

    public tGeometry ChassisGeometry => Memory.ClassFactory.Create<tGeometry>(ReadUInt32(8));

    public PointerArray<tShader> RefractionShaders => new(Memory, Address + 12, 16);

    public tShader HoodShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(76));

    public tShader TrunkShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(80));

    public tShader DoorPShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(84));

    public tShader DoorDShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(88));

    public Texture HoodTextureDam => Memory.ClassFactory.Create<Texture>(ReadUInt32(92));

    public Texture TrunkTextureDam => Memory.ClassFactory.Create<Texture>(ReadUInt32(96));

    public Texture DoorPTextureDam => Memory.ClassFactory.Create<Texture>(ReadUInt32(100));

    public Texture DoorDTextureDam => Memory.ClassFactory.Create<Texture>(ReadUInt32(104));

    public Texture HoodTextureNorm => Memory.ClassFactory.Create<Texture>(ReadUInt32(108));

    public Texture TrunkTextureNorm => Memory.ClassFactory.Create<Texture>(ReadUInt32(112));

    public Texture DoorPTextureNorm => Memory.ClassFactory.Create<Texture>(ReadUInt32(116));

    public Texture DoorDTextureNorm => Memory.ClassFactory.Create<Texture>(ReadUInt32(120));

    public tShader ChassisShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(124));

    public Texture ChassisTextureNorm => Memory.ClassFactory.Create<Texture>(ReadUInt32(128));

    public Texture ChassisTextureDam => Memory.ClassFactory.Create<Texture>(ReadUInt32(132));

    public VehicleParticleEmitter ParticleEmitter => Memory.ClassFactory.Create<VehicleParticleEmitter>(ReadUInt32(136));

    public ParticleAttributes EngineParticleAttr
    {
        get => ReadStruct<ParticleAttributes>(140);
        set => WriteStruct(140, value);
    }

    public ParticleAttributes LeftWheelParticleAttr
    {
        get => ReadStruct<ParticleAttributes>(160);
        set => WriteStruct(140, value);
    }

    public ParticleAttributes RightWheelParticleAttr
    {
        get => ReadStruct<ParticleAttributes>(180);
        set => WriteStruct(140, value);
    }

    public ParticleAttributes TailPipeParticleAttr
    {
        get => ReadStruct<ParticleAttributes>(200);
        set => WriteStruct(140, value);
    }

    public tParticleSystem VariableEmissionParticleSystem => Memory.ClassFactory.Create<tParticleSystem>(ReadUInt32(220));

    public SkidMarkGenerator SkidMarkGenerator => Memory.ClassFactory.Create<SkidMarkGenerator>(ReadUInt32(224));

    public tAnimation Anim => Memory.ClassFactory.Create<tAnimation>(ReadUInt32(220));

    public float AnimRevPerSecondBase
    {
        get => ReadSingle(232);
        set => WriteSingle(232, value);
    }

    public float RevMult
    {
        get => ReadSingle(236);
        set => WriteSingle(236, value);
    }

    public ParticleEnums.ParticleID SpecialEffect
    {
        get => (ParticleEnums.ParticleID)ReadInt32(240);
        set => WriteInt32(240, (int)value);
    }

    public Vector3 LastPosition
    {
        get => ReadStruct<Vector3>(244);
        set => WriteStruct(244, value);
    }

    public float CurEnvMapRotation
    {
        get => ReadSingle(256);
        set => WriteSingle(256, value);
    }

    public StructArray<int> BrakeLightJoints => new(Memory, Address + 260, sizeof(int), 4);

    public StructArray<int> ReverseLightJoints => new(Memory, Address + 276, sizeof(int), 4);

    public TrafficBodyDrawable TrafficBodyDrawable => Memory.ClassFactory.Create<TrafficBodyDrawable>(ReadUInt32(292));

    public TrafficBodyDrawable TrafficDoorDrawable => Memory.ClassFactory.Create<TrafficBodyDrawable>(ReadUInt32(296));

    public StructArray<float> ShadowPointAdjustments => new(Memory, Address + 300, sizeof(float), 8);

    public int FadeAlpha
    {
        get => ReadInt32(332);
        set => WriteInt32(332, value);
    }

    public tBillboardQuadGroup FrinkArc => Memory.ClassFactory.Create<tBillboardQuadGroup>(ReadUInt32(336));

    public StructArray<Color> OriginalFrinkArcColour => new(Memory, Address + 340, sizeof(int), 3);

    public PointerArray<tBillboardQuadGroup> BrakeLights => new(Memory, Address + 352, 4);

    public StructArray<Color> OriginalBrakeLightColours => new(Memory, Address + 368, sizeof(int), 4);

    public bool UsingTrafficModel
    {
        get => ReadBoolean(384);
        set => WriteBoolean(384, value);
    }

    public bool HasGhostGlow
    {
        get => ReadBoolean(385);
        set => WriteBoolean(385, value);
    }

    public PointerArray<tBillboardQuadGroup> GhostGlows => new(Memory, Address + 388, 6);

    public StructArray<Color> OriginalGhostGlowColours => new(Memory, Address + 412, sizeof(int), 6);

    public bool HasNukeGlow
    {
        get => ReadBoolean(436);
        set => WriteBoolean(436, value);
    }

    public PointerArray<tBillboardQuadGroup> NukeGlows => new(Memory, Address + 440, 1);

    public StructArray<Color> OriginalNukeGlowColours => new(Memory, Address + 444, sizeof(int), 3);

    public bool BrakeLightsOn
    {
        get => ReadBoolean(456);
        set => WriteBoolean(456, value);
    }

    public float BrakeLightScale
    {
        get => ReadSingle(460);
        set => WriteSingle(460, value);
    }

    public float HeadLightScale
    {
        get => ReadSingle(464);
        set => WriteSingle(464, value);
    }

    public bool EnableLights
    {
        get => ReadBoolean(468);
        set => WriteBoolean(468, value);
    }

    public bool LightsOffDueToDamage
    {
        get => ReadBoolean(469);
        set => WriteBoolean(469, value);
    }

    public DrawablePropElement RoofOpacShape => Memory.ClassFactory.Create<DrawablePropElement>(ReadUInt32(472));

    public DrawablePropElement RoofAlphaShape => Memory.ClassFactory.Create<DrawablePropElement>(ReadUInt32(476));

    public tShader RoofShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(480));

    public int RoofAlpha
    {
        get => ReadInt32(484);
        set => WriteInt32(484, value);
    }

    public int RoofTargetAlpha
    {
        get => ReadInt32(488);
        set => WriteInt32(488, value);
    }

    // TODO: std::vector< VehicleFrameController, s2alloc< VehicleFrameController > > mFrameControllers; (492)

    public StatePropCollectible Collectible => Memory.ClassFactory.Create<StatePropCollectible>(ReadUInt32(504));

    public Matrix4x4 CollectibleTransform
    {
        get => ReadStruct<Matrix4x4>(508);
        set => WriteStruct(508, value);
    }

    public byte EnvRef
    {
        get => ReadByte(572);
        set => WriteByte(572, value);
    }

    /// <summary>
    /// Sets the traffic body colour if applicable.
    /// </summary>
    /// <param name="Colour">
    /// The colour to set.
    /// </param>
    public void SetTrafficBodyColour(Color Colour)
    {
        TrafficBodyDrawable trafficBodyDrawable = TrafficBodyDrawable;
        if (trafficBodyDrawable != null)
            trafficBodyDrawable.DesiredColour = Colour;

        TrafficBodyDrawable trafficDoorDrawable = TrafficDoorDrawable;
        if (trafficDoorDrawable != null)
            trafficDoorDrawable.DesiredColour = Colour;
    }

    public void SetEngineSmoke(ParticleEnums.ParticleID pid)
    {
        ParticleAttributes particleAttributes = EngineParticleAttr;
        particleAttributes.Type = pid;
        EngineParticleAttr = particleAttributes;
    }

    public void DamangeTextureHood(bool on)
    {
        if (HoodTextureDam?.PDDITexture is not d3dTexture hoodTextureDamD3D || HoodTextureNorm?.PDDITexture is not d3dTexture hoodTextureNormD3D)
            return;

        pddiShader hoodShader = HoodShader?.PDDIShader;
        if (hoodShader is not d3dShader hoodShaderD3D)
            return;

        hoodShaderD3D.SetTexture(on ? hoodTextureDamD3D : hoodTextureNormD3D);
    }

    public void DamangeTextureTrunk(bool on)
    {
        if (TrunkTextureDam?.PDDITexture is not d3dTexture trunkTextureDamD3D || TrunkTextureNorm?.PDDITexture is not d3dTexture trunkTextureNormD3D)
            return;

        pddiShader trunkShader = TrunkShader?.PDDIShader;
        if (trunkShader is not d3dShader trunkShaderD3D)
            return;

        trunkShaderD3D.SetTexture(on ? trunkTextureDamD3D : trunkTextureNormD3D);
    }

    public void DamangeTextureDoorP(bool on)
    {
        if (DoorPTextureDam?.PDDITexture is not d3dTexture doorPTextureDamD3D || DoorPTextureNorm?.PDDITexture is not d3dTexture doorPTextureNormD3D)
            return;

        pddiShader doorPShader = DoorPShader?.PDDIShader;
        if (doorPShader is not d3dShader doorPShaderD3D)
            return;

        doorPShaderD3D.SetTexture(on ? doorPTextureDamD3D : doorPTextureNormD3D);
    }

    public void DamangeTextureDoorD(bool on)
    {
        if (DoorDTextureDam?.PDDITexture is not d3dTexture doorDTextureDamD3D || DoorDTextureNorm?.PDDITexture is not d3dTexture doorDTextureNormD3D)
            return;

        pddiShader doorDShader = DoorDShader?.PDDIShader;
        if (doorDShader is not d3dShader doorDShaderD3D)
            return;

        doorDShaderD3D.SetTexture(on ? doorDTextureDamD3D : doorDTextureNormD3D);
    }
}
