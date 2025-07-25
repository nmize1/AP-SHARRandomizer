using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System;
using System.Drawing;
using System.Text;
using static SHARMemory.SHAR.Globals;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVVehicle@@")]
public class Vehicle : DynaPhysDSG
{
    public enum DamageTypes
    {
        Unset,
        User,
        AI,
        Traffic
    }

    public enum Door
    {
        Driver,
        Passenger
    };

    public enum DoorAction
    {
        None,
        Open,
        Close
    };

    public enum VehicleLocomotionTypes
    {
        Physics,
        Traffic
    }

    public enum VehicleStates
    {
        Normal,
        Slip,
        EBrake_Slip
    }

    public enum VehicleTypes
    {
        User,
        AI,
        Traffic,
        Last
    }

    public Vehicle(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public override Vector3 Position
    {
        get
        {
            Matrix4x4 transform = Transform;
            return new(transform.M41, transform.M42, transform.M43);
        }
        set
        {
            Matrix4x4 transform = Transform;
            transform.M41 = value.X;
            transform.M42 = value.Y;
            transform.M43 = value.Z;
            Transform = transform;
        }
    }

    public bool AlreadyCalledAutoResetOnSpot
    {
        get => ReadBoolean(160);
        set => WriteBoolean(160, value);
    }

    public VehicleTypes VehicleType
    {
        get => (VehicleTypes)ReadUInt32(164);
        set => WriteUInt32(164, (uint)value);
    }

    public int VehicleCentralIndex
    {
        get => ReadInt32(168);
        set => WriteInt32(168, value);
    }

    public float SecondsTillNextTurbo
    {
        get => ReadSingle(172);
        set => WriteSingle(172, value);
    }

    public int NumTurbos
    {
        get => ReadInt32(176);
        set => WriteInt32(176, value);
    }

    public GeometryVehicle GeometryVehicle => Memory.ClassFactory.Create<GeometryVehicle>(ReadUInt32(180));

    public Matrix4x4 Transform
    {
        get => ReadStruct<Matrix4x4>(184);
        set => WriteStruct(184, value);
    }

    public new string Name => ReadNullStringPointer(248, Encoding.UTF8);

    public Vector3 InitialPosition
    {
        get => ReadStruct<Vector3>(252);
        set => WriteStruct(252, value);
    }

    public float ResetFacingRadians
    {
        get => ReadSingle(264);
        set => WriteSingle(264, value);
    }

    public uint VehicleID
    {
        get => ReadUInt32(268);
        set => WriteUInt32(268, value);
    }

    public VehicleEventListener VehicleEventListener => Memory.ClassFactory.Create<VehicleEventListener>(ReadUInt32(272));

    public bool DoingJumpBoost
    {
        get => ReadBoolean(276);
        set => WriteBoolean(276, value);
    }

    public Vector3 VehicleFacing
    {
        get => ReadStruct<Vector3>(280);
        set => WriteStruct(280, value);
    }

    public Vector3 VehicleUp
    {
        get => ReadStruct<Vector3>(292);
        set => WriteStruct(292, value);
    }

    public Vector3 VehicleTransverse
    {
        get => ReadStruct<Vector3>(304);
        set => WriteStruct(304, value);
    }

    public Vector3 VelocityCM
    {
        get => ReadStruct<Vector3>(316);
        set => WriteStruct(316, value);
    }

    public float Speed
    {
        get => ReadSingle(328);
        set => WriteSingle(328, value);
    }

    public float PercentOfTopSpeed
    {
        get => ReadSingle(332);
        set => WriteSingle(332, value);
    }

    public float SpeedKmh
    {
        get => ReadSingle(336);
        set => WriteSingle(336, value);
    }

    public float LastSpeedKmh
    {
        get => ReadSingle(340);
        set => WriteSingle(340, value);
    }

    public float AccelMss
    {
        get => ReadSingle(344);
        set => WriteSingle(344, value);
    }

    public Vector3 OriginalCMOffset
    {
        get => ReadStruct<Vector3>(348);
        set => WriteStruct(348, value);
    }

    public Vector3 CMOffset
    {
        get => ReadStruct<Vector3>(360);
        set => WriteStruct(360, value);
    }

    public IntersectManager.TerrainType TerrainType
    {
        get => (IntersectManager.TerrainType)ReadUInt32(372);
        set => WriteUInt32(372, (uint)value);
    }

    public bool Interior
    {
        get => ReadBoolean(376);
        set => WriteBoolean(376, value);
    }

    public VehicleStates VehicleState
    {
        get => (VehicleStates)ReadUInt32(380);
        set => WriteUInt32(380, (uint)value);
    }

    public VehicleLocomotion VehicleLocomotion => Memory.ClassFactory.Create<VehicleLocomotion>(ReadUInt32(384));

    public VehicleLocomotionTypes VehicleLocomotionType
    {
        get => (VehicleLocomotionTypes)ReadUInt32(388);
        set => WriteUInt32(388, (uint)value);
    }

    public PhysicsLocomotion PhysicsLocomotion => Memory.ClassFactory.Create<PhysicsLocomotion>(ReadUInt32(392));

    public TrafficVehicle TrafficVehicle => Memory.ClassFactory.Create<TrafficVehicle>(ReadUInt32(396));

    public TrafficLocomotion TrafficLocomotion => Memory.ClassFactory.Create<TrafficLocomotion>(ReadUInt32(400));

    public bool LocoSwitchedToPhysicsThisFrame
    {
        get => ReadBoolean(404);
        set => WriteBoolean(404, value);
    }

    public int CollisionAreaIndex
    {
        get => ReadInt32(408);
        set => WriteInt32(408, value);
    }

    public RenderEnums.LayerEnum RenderLayerEnum
    {
        get => (RenderEnums.LayerEnum)ReadInt32(412);
        set => WriteInt32(412, (int)value);
    }

    public bool OkToDrawSelf
    {
        get => ReadBoolean(416);
        set => WriteBoolean(416, value);
    }

    public bool DrawVehicle
    {
        get => ReadBoolean(417);
        set => WriteBoolean(417, value);
    }

    public float Gas
    {
        get => ReadSingle(420);
        set => WriteSingle(420, value);
    }

    public float LastGas
    {
        get => ReadSingle(424);
        set => WriteSingle(424, value);
    }

    public float DeltaGas
    {
        get => ReadSingle(428);
        set => WriteSingle(428, value);
    }

    public float Brake
    {
        get => ReadSingle(432);
        set => WriteSingle(432, value);
    }

    public float WheelTurnAngle
    {
        get => ReadSingle(436);
        set => WriteSingle(436, value);
    }

    public float WheelTurnAngleInputValue
    {
        get => ReadSingle(440);
        set => WriteSingle(440, value);
    }

    public float Reverse
    {
        get => ReadSingle(444);
        set => WriteSingle(444, value);
    }

    public float EBrake
    {
        get => ReadSingle(448);
        set => WriteSingle(448, value);
    }

    public float EBrakeTimer
    {
        get => ReadSingle(452);
        set => WriteSingle(452, value);
    }

    public bool BrakeLightsOn
    {
        get => ReadBoolean(456);
        set => WriteBoolean(456, value);
    }

    public bool ReverseLightsOn
    {
        get => ReadBoolean(457);
        set => WriteBoolean(457, value);
    }

    public float BrakeTimer
    {
        get => ReadSingle(460);
        set => WriteSingle(460, value);
    }

    public bool BrakeActingAsReverse
    {
        get => ReadBoolean(464);
        set => WriteBoolean(464, value);
    }

    public float SteeringInputThreshold
    {
        get => ReadSingle(468);
        set => WriteSingle(468, value);
    }

    public float SteeringPreSlope
    {
        get => ReadSingle(472);
        set => WriteSingle(472, value);
    }

    public float UnmodifiedInputWheelTurnAngle
    {
        get => ReadSingle(476);
        set => WriteSingle(476, value);
    }

    public bool DoingRockford
    {
        get => ReadBoolean(480);
        set => WriteBoolean(480, value);
    }

    public float SpeedBurstTimer
    {
        get => ReadSingle(484);
        set => WriteSingle(484, value);
    }

    public bool BuildingUpSpeedBurst
    {
        get => ReadBoolean(488);
        set => WriteBoolean(488, value);
    }

    public bool DoSpeedBurst
    {
        get => ReadBoolean(489);
        set => WriteBoolean(489, value);
    }

    public float FOVToRestore
    {
        get => ReadSingle(492);
        set => WriteSingle(492, value);
    }

    public float SpeedBurstTimeHalf
    {
        get => ReadSingle(496);
        set => WriteSingle(496, value);
    }

    public bool GasBrakeDisabled
    {
        get => ReadBoolean(500);
        set => WriteBoolean(500, value);
    }

    public bool SteeringWheelsOutOfContact
    {
        get => ReadBoolean(501);
        set => WriteBoolean(501, value);
    }

    public bool AirBorn
    {
        get => ReadBoolean(502);
        set => WriteBoolean(502, value);
    }

    public bool WeebleOn
    {
        get => ReadBoolean(503);
        set => WriteBoolean(503, value);
    }

    public bool CMOffsetSetToOriginal
    {
        get => ReadBoolean(504);
        set => WriteBoolean(504, value);
    }

    public bool InstabilityOffsetOn
    {
        get => ReadBoolean(505);
        set => WriteBoolean(505, value);
    }

    public string DriverName
    {
        get => ReadString(506, Encoding.UTF8, 32);
        set => WriteString(506, value, Encoding.UTF8, 32);
    }

    public Character Driver => Memory.ClassFactory.Create<Character>(ReadUInt32(540));

    public bool PhantomDriver
    {
        get => ReadBoolean(544);
        set => WriteBoolean(544, value);
    }

    public Vector3 OurRestSeatingPosition
    {
        get => ReadStruct<Vector3>(548);
        set => WriteStruct(548, value);
    }

    public Vector3 NPCRestSeatingPosition
    {
        get => ReadStruct<Vector3>(560);
        set => WriteStruct(560, value);
    }

    public float YAccelForSeatingOffset
    {
        get => ReadSingle(572);
        set => WriteSingle(572, value);
    }

    public float MaxBounceDisplacementPerSecond
    {
        get => ReadSingle(576);
        set => WriteSingle(576, value);
    }

    public float BounceAccelThreshold
    {
        get => ReadSingle(580);
        set => WriteSingle(580, value);
    }

    public Vector3 VelocityCMLag
    {
        get => ReadStruct<Vector3>(584);
        set => WriteStruct(584, value);
    }

    public Vector3 PositionCMLag
    {
        get => ReadStruct<Vector3>(596);
        set => WriteStruct(596, value);
    }

    public float BounceLimit
    {
        get => ReadSingle(608);
        set => WriteSingle(608, value);
    }

    public bool UserDrivingCar
    {
        get => ReadBoolean(612);
        set => WriteBoolean(612, value);
    }

    public float RPM
    {
        get => ReadSingle(616);
        set => WriteSingle(616, value);
    }

    public float RPMUpRate
    {
        get => ReadSingle(620);
        set => WriteSingle(620, value);
    }

    public float RPMDownRate
    {
        get => ReadSingle(624);
        set => WriteSingle(624, value);
    }

    public float BaseRPM
    {
        get => ReadSingle(628);
        set => WriteSingle(628, value);
    }

    public float MaxRpm
    {
        get => ReadSingle(632);
        set => WriteSingle(632, value);
    }

    public float ShiftPointHigh
    {
        get => ReadSingle(636);
        set => WriteSingle(636, value);
    }

    public float ShiftPointLow
    {
        get => ReadSingle(640);
        set => WriteSingle(640, value);
    }

    public int Gear
    {
        get => ReadInt32(644);
        set => WriteInt32(644, value);
    }

    public float SkidLevel
    {
        get => ReadSingle(648);
        set => WriteSingle(648, value);
    }

    public float BurnoutLevel
    {
        get => ReadSingle(652);
        set => WriteSingle(652, value);
    }

    public bool DoingBurnout
    {
        get => ReadBoolean(656);
        set => WriteBoolean(656, value);
    }

    public bool NoSkid
    {
        get => ReadBoolean(657);
        set => WriteBoolean(657, value);
    }

    public bool NoFrontSkid
    {
        get => ReadBoolean(658);
        set => WriteBoolean(658, value);
    }

    public bool DoingWheelie
    {
        get => ReadBoolean(659);
        set => WriteBoolean(659, value);
    }

    public int NumGears
    {
        get => ReadInt32(660);
        set => WriteInt32(660, value);
    }

    public StructArray<float> GearRatios => new(Memory, ReadUInt32(664), sizeof(float), NumGears);

    public float FinalDriveRatio
    {
        get => ReadSingle(668);
        set => WriteSingle(668, value);
    }

    public VehicleDesignerParams DesignerParams
    {
        get => ReadStruct<VehicleDesignerParams>(672);
        set => WriteStruct(672, DesignerParams);
    }

    public bool PlayerCar
    {
        get => ReadBoolean(796);
        set => WriteBoolean(796, value);
    }

    public StructArray<bool> DamperShouldNotPullDown => new(Memory, Address + 797, sizeof(bool), 4);

    public PointerArray<SuspensionJointDriver> SuspensionJointDrivers => new(Memory, Address + 804, 4);

    public StructArray<int> WheelToJointIndexMapping => new(Memory, Address + 820, sizeof(int), 4);

    public StructArray<int> JointIndexToWheelMapping => new(Memory, ReadUInt32(836), sizeof(int), GeometryVehicle.CompositeDrawable?.Pose?.NumJoints ?? 0);

    public StructArray<Vector3> SuspensionRestPointsFromFile => new(Memory, Address + 840, Vector3.Size, 4);

    public StructArray<Vector3> SuspensionRestPoints => new(Memory, Address + 888, Vector3.Size, 4);

    public float SuspensionRestValue
    {
        get => ReadSingle(936);
        set => WriteSingle(936, value);
    }

    public float SuspensionMaxValue
    {
        get => ReadSingle(940);
        set => WriteSingle(940, value);
    }

    public StructArray<Vector3> SuspensionWorldSpacePoints => new(Memory, Address + 944, Vector3.Size, 4);

    public StructArray<Vector3> SuspensionPointVelocities => new(Memory, Address + 992, Vector3.Size, 4);

    public float WheelBase
    {
        get => ReadSingle(1040);
        set => WriteSingle(1040, value);
    }

    public PointerArray<Wheel> Wheels => new(Memory, Address + 1044, 4);

    public DamageTypes DamageType
    {
        get => (DamageTypes)ReadInt32(1060);
        set => WriteInt32(1060, (int)value);
    }

    public PointerArray<PhysicsJointInertialEffector> InertialJointDrivers => new(Memory, Address + 1064, 4);

    public StructArray<int> JointIndexToInertialJointDriverMapping => new(Memory, ReadUInt32(1080), sizeof(int), GeometryVehicle.CompositeDrawable?.Pose?.NumJoints ?? 0);

    public PointerArray<PhysicsJointMatrixModifier> PhysicsJointMatrixModifiers => new(Memory, Address + 1084, 4);

    public int DoorDJoint
    {
        get => ReadInt32(1100);
        set => WriteInt32(1100, value);
    }

    public int DoorPJoint
    {
        get => ReadInt32(1104);
        set => WriteInt32(1104, value);
    }

    public int HoodJoint
    {
        get => ReadInt32(1108);
        set => WriteInt32(1108, value);
    }

    public int TrunkJoint
    {
        get => ReadInt32(1112);
        set => WriteInt32(1112, value);
    }

    public int DoorDDamageLevel
    {
        get => ReadInt32(1116);
        set => WriteInt32(1116, value);
    }

    public int DoorPDamageLevel
    {
        get => ReadInt32(1120);
        set => WriteInt32(1120, value);
    }

    public int HoodDamageLevel
    {
        get => ReadInt32(1124);
        set => WriteInt32(1124, value);
    }

    public int TrunkDamageLevel
    {
        get => ReadInt32(1128);
        set => WriteInt32(1128, value);
    }

    public bool CollidedWithVehicle
    {
        get => ReadBoolean(1132);
        set => WriteBoolean(1132, value);
    }

    public bool OutOfControl
    {
        get => ReadBoolean(1133);
        set => WriteBoolean(1133, value);
    }

    public float NormalizedMagnitudeOfVehicleHit
    {
        get => ReadSingle(1136);
        set => WriteSingle(1136, value);
    }

    public bool WasHitByVehicle
    {
        get => ReadBoolean(1140);
        set => WriteBoolean(1140, value);
    }

    public VehicleTypes WasHitByVehicleType
    {
        get => (VehicleTypes)ReadInt32(1144);
        set => WriteInt32(1144, (int)value);
    }

    public Vector3 SwerveNormal
    {
        get => ReadStruct<Vector3>(1148);
        set => WriteStruct(1148, value);
    }

    public Vector3 SmokeOffset
    {
        get => ReadStruct<Vector3>(1160);
        set => WriteStruct(1160, value);
    }

    public bool VehicleDestroyed
    {
        get => ReadBoolean(1172);
        set => WriteBoolean(1172, value);
    }

    public bool DontShowBrakeLights
    {
        get => ReadBoolean(1173);
        set => WriteBoolean(1173, value);
    }

    public bool AlreadyPlayedExplosion
    {
        get => ReadBoolean(1174);
        set => WriteBoolean(1174, value);
    }

    public float DamageOutResetTimer
    {
        get => ReadSingle(1176);
        set => WriteSingle(1176, value);
    }

    public float NoDamageTimer
    {
        get => ReadSingle(1180);
        set => WriteSingle(1180, value);
    }

    public float HitPoints
    {
        get => ReadSingle(1184);
        set => WriteSingle(1184, value);
    }

    public bool VehicleCanSustainDamage
    {
        get => ReadBoolean(1188);
        set => WriteBoolean(1188, value);
    }

    public bool IsADestroyObjective
    {
        get => ReadBoolean(1189);
        set => WriteBoolean(1189, value);
    }

    public SimStateArticulated SimStateArticulated => Memory.ClassFactory.Create<SimStateArticulated>(ReadUInt32(1192));

    public ArticulatedPhysicsObject ArticulatedPhysicsObject => Memory.ClassFactory.Create<ArticulatedPhysicsObject>(ReadUInt32(1196));

    public PhysicsProperties PhysicsProperties => Memory.ClassFactory.Create<PhysicsProperties>(ReadUInt32(1200));

    public SimStateArticulated SimStateArticulatedOutOfCar => Memory.ClassFactory.Create<SimStateArticulated>(ReadUInt32(1204));

    public SimStateArticulated SimStateArticulatedInCar => Memory.ClassFactory.Create<SimStateArticulated>(ReadUInt32(1208));

    public bool UsingInCarPhysics
    {
        get => ReadBoolean(1212);
        set => WriteBoolean(1212, value);
    }

    public ManualSimState GroundPlaneSimState => Memory.ClassFactory.Create<ManualSimState>(ReadUInt32(1216));

    public WallVolume GroundPlaneWallVolume => Memory.ClassFactory.Create<WallVolume>(ReadUInt32(1220));

    public PhysicsProperties GroundPlanePhysicsProperties => Memory.ClassFactory.Create<PhysicsProperties>(ReadUInt32(1224));

    public bool AtRestAsFarAsTriggersAreConcerned
    {
        get => ReadBoolean(1228);
        set => WriteBoolean(1228, value);
    }

    public bool CreatedByParkedCarManager
    {
        get => ReadBoolean(1229);
        set => WriteBoolean(1229, value);
    }

    public PoseEngine PoseEngine => Memory.ClassFactory.Create<PoseEngine>(ReadUInt32(1232));

    public RootMatrixDriver RootMatrixDriver => Memory.ClassFactory.Create<RootMatrixDriver>(ReadUInt32(1236));

    public float DragCoeff
    {
        get => ReadSingle(1240);
        set => WriteSingle(1240, value);
    }

    public float RollingFrictionForce
    {
        get => ReadSingle(1244);
        set => WriteSingle(1244, value);
    }

    public float TireLateralResistance
    {
        get => ReadSingle(1248);
        set => WriteSingle(1248, value);
    }

    public float SlipGasModifier
    {
        get => ReadSingle(1252);
        set => WriteSingle(1252, value);
    }

    public float CollisionLateralResistanceDropFactor
    {
        get => ReadSingle(1256);
        set => WriteSingle(1256, value);
    }

    public bool BottomedOutThisFrame
    {
        get => ReadBoolean(1260);
        set => WriteBoolean(1260, value);
    }

    public bool WasAirborn
    {
        get => ReadBoolean(1261);
        set => WriteBoolean(1261, value);
    }

    public float WasAirbornTimer
    {
        get => ReadSingle(1264);
        set => WriteSingle(1264, value);
    }

    public float BottomOutSpeedMaintenance
    {
        get => ReadSingle(1268);
        set => WriteSingle(1268, value);
    }

    public float StuckOnSideTimer
    {
        get => ReadSingle(1272);
        set => WriteSingle(1272, value);
    }

    public bool DrawWireFrame
    {
        get => ReadBoolean(1276);
        set => WriteBoolean(1276, value);
    }

    public bool LosingTractionDueToAccel
    {
        get => ReadBoolean(1277);
        set => WriteBoolean(1277, value);
    }

    public RectTriggerVolume TriggerVolume => Memory.ClassFactory.Create<RectTriggerVolume>(ReadUInt32(1280));

    public EventLocator EventLocator => Memory.ClassFactory.Create<EventLocator>(ReadUInt32(1284));

    public bool TriggerActive
    {
        get => ReadBoolean(1288);
        set => WriteBoolean(1288, value);
    }

    public int DriverInit
    {
        get => ReadInt32(1292);
        set => WriteInt32(1292, value);
    }

    public bool HijackedByUser
    {
        get => ReadBoolean(1296);
        set => WriteBoolean(1296, value);
    }

    public float ForceToDetachCollectible
    {
        get => ReadSingle(1300);
        set => WriteSingle(1300, value);
    }

    public int CharacterSheetCarIndex
    {
        get => ReadInt32(1304);
        set => WriteInt32(1304, value);
    }

    private byte Bitfield_0x51C
    {
        get => ReadByte(1308);
        set => WriteByte(1308, value);
    }

    public bool HasDoors
    {
        get => (Bitfield_0x51C & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield_0x51C |= 0b00000001;
            else
                Bitfield_0x51C &= 0b11111110;
        }
    }

    public bool VisibleCharacters
    {
        get => (Bitfield_0x51C & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield_0x51C |= 0b00000010;
            else
                Bitfield_0x51C &= 0b11111101;
        }
    }

    public bool IrisTransition
    {
        get => (Bitfield_0x51C & 0b00000100) != 0;
        set
        {
            if (value)
                Bitfield_0x51C |= 0b00000100;
            else
                Bitfield_0x51C &= 0b11111011;
        }
    }

    public bool AllowSlide
    {
        get => (Bitfield_0x51C & 0b00001000) != 0;
        set
        {
            if (value)
                Bitfield_0x51C |= 0b00001000;
            else
                Bitfield_0x51C &= 0b11110111;
        }
    }

    public bool HighRoof
    {
        get => (Bitfield_0x51C & 0b00010000) != 0;
        set
        {
            if (value)
                Bitfield_0x51C |= 0b00010000;
            else
                Bitfield_0x51C &= 0b11101111;
        }
    }

    public float CharacterScale
    {
        get => ReadSingle(1312);
        set => WriteSingle(1312, value);
    }

    public StructArray<float> DesiredDoorPosition => new(Memory, Address + 1316, sizeof(float), 2);

    public StructArray<int> DesiredDoorAction => new(Memory, Address + 1324, sizeof(int), 2);

    public Vector3 PassengerLocation
    {
        get => ReadStruct<Vector3>(1332);
        set => WriteStruct(1332, value);
    }

    public Vector3 DriverLocation
    {
        get => ReadStruct<Vector3>(1344);
        set => WriteStruct(1344, value);
    }

    public Vector3 Extents
    {
        get => ReadStruct<Vector3>(1356);
        set => WriteStruct(1356, value);
    }

    public bool IsSimpleShadow
    {
        get => ReadBoolean(1368);
        set => WriteBoolean(1368, value);
    }

    public void EnableWonkyDriving()
    {
        var articulatedPhyicsObject = ArticulatedPhysicsObject;
        if (articulatedPhyicsObject == null)
            return;

        articulatedPhyicsObject.TimeComputeInertiaMatrix = float.NaN;

        var initialInertiaMatrix = articulatedPhyicsObject.InitialInertiaMatrix;
        if (initialInertiaMatrix != null)
        {
            initialInertiaMatrix.Identity();
            initialInertiaMatrix.Scale(articulatedPhyicsObject.Mass);
        }

        var inertiaMatrix = articulatedPhyicsObject.InertiaMatrix;
        if (inertiaMatrix != null)
        {
            inertiaMatrix.Identity();
            inertiaMatrix.Scale(articulatedPhyicsObject.Mass);
        }
    }

    public void FixWonkyDrivingAndTilt()
    {
        ArticulatedPhysicsObject articulatedPhyicsObject = ArticulatedPhysicsObject;
        if (articulatedPhyicsObject == null)
            return;

        articulatedPhyicsObject.TimeComputeInertiaMatrix = 0.1f;
    }

    /// <summary>
    /// Stops all vehicle momentum.
    /// </summary>
    public void Stop()
    {
        SimStateArticulated simStateArticulated = SimStateArticulated;
        if (simStateArticulated == null)
            return;

        SimVelocityState simVelocityState = simStateArticulated.VelocityState;
        simVelocityState.Linear = new(0);
        simVelocityState.Angular = new(0);
        simStateArticulated.VelocityState = simVelocityState;
        if (SimStateArticulated.SimulatedObject is not PhysicsObject physicsObject)
            return;
        physicsObject.AngularMomentum = new(0);
    }

    /// <summary>
    /// Launches the vehicle upwards and in the direction it's facing
    /// </summary>
    /// <param name="Height">
    /// How high to send the vehicle. Defaults to <c>20</c>.
    /// </param>
    /// <param name="LaunchPower">
    /// How hard to launch the vehicle forwards. Defaults to <c>5</c>.
    /// </param>
    public void Launch(float Height = 20, float LaunchPower = 5)
    {
        SimStateArticulated simStateArticulated = SimStateArticulated;
        if (simStateArticulated == null)
            return;
        
        SimVelocityState simVelocityState = simStateArticulated.VelocityState;
        Vector3 linear = Vector3.Add(simVelocityState.Linear, VehicleFacing * LaunchPower);
        linear.Y += Height;
        simVelocityState.Linear = linear;
        simStateArticulated.VelocityState = simVelocityState;
    }

    /// <summary>
    /// Sets the traffic body colour if applicable.
    /// </summary>
    /// <param name="Colour">
    /// The colour to set.
    /// </param>
    public void SetTrafficBodyColour(Color Colour) => GeometryVehicle?.SetTrafficBodyColour(Colour);

    public double GetFacingInRadians()
    {
        Vector3 facing = VehicleFacing;
        double test = Math.Atan2(facing.X, facing.Z);

        if (double.IsNaN(test))
            return 0;

        return test;
    }

    public void SetTransform(Matrix4x4 matrix)
    {
        SimStateArticulated simStateArticulated = SimStateArticulated;
        if (simStateArticulated == null)
            return;

        if (simStateArticulated.Control == SimState.SimControlEnum.AICtrl)
        {
            simStateArticulated.SetTransform(matrix);
        }
        else
        {
            simStateArticulated.SetControl(SimState.SimControlEnum.AICtrl);
            simStateArticulated.SetTransform(matrix);
            simStateArticulated.SetControl(SimState.SimControlEnum.SimulationCtrl);
        }
    }

    public void ResetFlagsOnly(bool resetDamage)
    {
        OkToDrawSelf = true;

        DoingBurnout = false;

        DoingRockford = false;

        SpeedBurstTimer = 0f;
        EBrakeTimer = 0f;
        BuildingUpSpeedBurst = false;
        DoSpeedBurst = false;
        DoingJumpBoost = false;
        BrakeLightsOn = false;
        ReverseLightsOn = false;
        CMOffsetSetToOriginal = false;
        StuckOnSideTimer = 0f;

        if (resetDamage)
        {
            VehicleDestroyed = false;
            ResetDamageState();
            AlreadyPlayedExplosion = false;
        }

        DesiredDoorPosition.FromArray([0f, 0f]);
        DesiredDoorAction.FromArray([(int)DoorAction.None, (int)DoorAction.None]);

        DrawWireFrame = false;

        WasAirborn = false;
        WasAirbornTimer = 0f;

        BottomOutSpeedMaintenance = 0f;

        if (VehicleType == VehicleTypes.AI && VehicleCentralIndex != -1)
        {
            // TODO: Reset AI controller
        }
    }

    public void ResetDamageState()
    {
        GeometryVehicle geometryVehicle = GeometryVehicle;
        VehicleDestroyed = false;
        DontShowBrakeLights = false;
        if (geometryVehicle != null)
            geometryVehicle.LightsOffDueToDamage = false;
        DamageOutResetTimer = 0f;
        HitPoints = DesignerParams.HitPoints;

        DesiredDoorPosition.FromArray([0f, 0f]);
        DesiredDoorAction.FromArray([(int)DoorAction.None, (int)DoorAction.None]);

        // TODO: ActivateTriggers(true)

        if (PlayerCar)
        {
            // TODO: Memory.Singletons.CharacterSheetManager->UpdateCarHealth(CharacterSheetCarIndex, 1f);
        }

        switch (DamageType)
        {
            case DamageTypes.User:
                ArticulatedPhysicsObject articulatedPhysicsObject = ArticulatedPhysicsObject;
                if (articulatedPhysicsObject != null)
                {
                    int[] JointIndexToInertialJointDriverMapping = [.. this.JointIndexToInertialJointDriverMapping];
                    if (HoodJoint != -1)
                    {
                        int index = JointIndexToInertialJointDriverMapping[HoodJoint];
                        if (index != -1)
                        {
                            PhysicsJoint physicsJoint = articulatedPhysicsObject.Joints[HoodJoint];
                            if (physicsJoint != null)
                            {
                                if (physicsJoint.NumDOF > 0)
                                    physicsJoint.InverseStiffness = 0f;
                                physicsJoint.ResetDeformation();
                            }
                            PhysicsJointInertialEffector physicsJointInertialEffector = InertialJointDrivers[index];
                            if (physicsJointInertialEffector != null)
                                physicsJointInertialEffector.Enabled = false;
                        }
                    }

                    if (TrunkJoint != -1)
                    {
                        int index = JointIndexToInertialJointDriverMapping[TrunkJoint];
                        if (index != -1)
                        {
                            PhysicsJoint physicsJoint = articulatedPhysicsObject.Joints[TrunkJoint];
                            if (physicsJoint != null)
                            {
                                if (physicsJoint.NumDOF > 0)
                                    physicsJoint.InverseStiffness = 0f;
                                physicsJoint.ResetDeformation();
                            }
                            PhysicsJointInertialEffector physicsJointInertialEffector = InertialJointDrivers[index];
                            if (physicsJointInertialEffector != null)
                                physicsJointInertialEffector.Enabled = false;
                        }
                    }

                    if (DoorDJoint != -1)
                    {
                        int index = JointIndexToInertialJointDriverMapping[DoorDJoint];
                        if (index != -1)
                        {
                            PhysicsJoint physicsJoint = articulatedPhysicsObject.Joints[DoorDJoint];
                            if (physicsJoint != null)
                            {
                                if (physicsJoint.NumDOF > 0)
                                    physicsJoint.InverseStiffness = 0f;
                                physicsJoint.ResetDeformation();
                            }
                            PhysicsJointInertialEffector physicsJointInertialEffector = InertialJointDrivers[index];
                            if (physicsJointInertialEffector != null)
                                physicsJointInertialEffector.Enabled = false;
                        }
                    }

                    if (DoorPJoint != -1)
                    {
                        int index = JointIndexToInertialJointDriverMapping[DoorPJoint];
                        if (index != -1)
                        {
                            PhysicsJoint physicsJoint = articulatedPhysicsObject.Joints[DoorPJoint];
                            if (physicsJoint != null)
                            {
                                if (physicsJoint.NumDOF > 0)
                                    physicsJoint.InverseStiffness = 0f;
                                physicsJoint.ResetDeformation();
                            }
                            PhysicsJointInertialEffector physicsJointInertialEffector = InertialJointDrivers[index];
                            if (physicsJointInertialEffector != null)
                                physicsJointInertialEffector.Enabled = false;
                        }
                    }
                }

                if (geometryVehicle != null)
                {
                    geometryVehicle.DamangeTextureHood(false);
                    geometryVehicle.DamangeTextureTrunk(false);
                    geometryVehicle.DamangeTextureDoorD(false);
                    geometryVehicle.DamangeTextureDoorP(false);
                    geometryVehicle.SetEngineSmoke(ParticleEnums.ParticleID.Null);
                }
                break;
            case DamageTypes.AI:
                if (geometryVehicle != null)
                {
                    geometryVehicle.DamangeTextureHood(false);
                    geometryVehicle.DamangeTextureTrunk(false);
                    geometryVehicle.DamangeTextureDoorD(false);
                    geometryVehicle.DamangeTextureDoorP(false);
                    geometryVehicle.SetEngineSmoke(ParticleEnums.ParticleID.Null);
                }
                break;
            case DamageTypes.Traffic:
                geometryVehicle?.SetEngineSmoke(ParticleEnums.ParticleID.Null);
                break;
        }
    }
}
