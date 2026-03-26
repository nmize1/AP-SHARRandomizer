local Path = GetPath()
local GamePath = "/GameData/" .. Path
local P3DFile = P3D.P3DFile(GamePath)

local level = GetCurrentLevel()

local SkeletonChunk = P3DFile:GetChunk(P3D.Identifiers.Skeleton, true, "level1_switch")
local AnimCollChunk = P3DFile:GetChunk(P3D.Identifiers.Anim_Coll, true, "level1_switch")
local LocatorChunk = P3DFile:GetChunk(P3D.Identifiers.Locator, true, "Level1")
local TriggerVolume = LocatorChunk:GetChunk(P3D.Identifiers.Trigger_Volume)
local LocatorMatrix = LocatorChunk:GetChunk(P3D.Identifiers.Locator_Matrix)

local SkeletonJoint = SkeletonChunk.Chunks[1]
local restPose = SkeletonJoint.RestPose

local angle = math.rad(45)
local cosA = math.cos(angle)
local sinA = math.sin(angle)
local coords = {
	[1] = {
			x = 218.6, y = 3.5, z = -180.2, 
			m11 = -1,  m12 = 0, m13 = 0,
			m21 = 0,   m22 = 1, m23 = 0,
			m31 = 0,   m32 = 0, m33 = -1
		  },
	[2] = {
			x = 139, y = 8, z = -34,
			m11 = -1,  m12 = 0, m13 = 0,
			m21 = 0,   m22 = 1, m23 = 0,
			m31 = 0,   m32 = 0, m33 = -1
		  },
	[3] = {
			x = 26, y = 7, z = -2.5,
			m11 = -cosA, m12 = 0, m13 = -sinA,
			m21 = 0, m22 = 1, m23 = 0,
			m31 = sinA, m32 = 0, m33 = -cosA
		  },
	[4] = {
			x = 218.6, y = 3.5, z = -180.2,
			m11 = -1,  m12 = 0, m13 = 0,
			m21 = 0,   m22 = 1, m23 = 0,
			m31 = 0,   m32 = 0, m33 = -1
		  },
	[5] = {
			x = -141, y = 29, z = 437.9,
			m11 = -1,  m12 = 0, m13 = 0,
			m21 = 0,   m22 = 1, m23 = 0,
			m31 = 0,   m32 = 0, m33 = -1
		  },
	[6] = {
			x = 26, y = 7, z = -2.5,
			m11 = -cosA, m12 = 0, m13 = -sinA,
			m21 = 0, m22 = 1, m23 = 0,
			m31 = sinA, m32 = 0, m33 = -cosA
		  },
	[7] = {
			x = 218.6, y = 3.5, z = -180.2, 
			m11 = -1,  m12 = 0, m13 = 0,
			m21 = 0,   m22 = 1, m23 = 0,
			m31 = 0,   m32 = 0, m33 = -1
		  }
}

local c = coords[level]

restPose.M11 = c.m11
restPose.M12 = c.m12
restPose.M13 = c.m13
restPose.M21 = c.m21
restPose.M22 = c.m22
restPose.M23 = c.m23
restPose.M31 = c.m31
restPose.M32 = c.m32
restPose.M33 = c.m33

restPose.M41 = c.x
restPose.M42 = c.y + 1
restPose.M43 = c.z

LocatorChunk.Position.X = c.x
LocatorChunk.Position.Y = c.y
LocatorChunk.Position.Z = c.z

TriggerVolume.Matrix.M11 = c.m11
TriggerVolume.Matrix.M12 = c.m12
TriggerVolume.Matrix.M13 = c.m13
TriggerVolume.Matrix.M21 = c.m21
TriggerVolume.Matrix.M22 = c.m22
TriggerVolume.Matrix.M23 = c.m23
TriggerVolume.Matrix.M31 = c.m31
TriggerVolume.Matrix.M32 = c.m32
TriggerVolume.Matrix.M33 = c.m33

TriggerVolume.Matrix.M41 = c.x
TriggerVolume.Matrix.M42 = c.y
TriggerVolume.Matrix.M43 = c.z

LocatorMatrix.Matrix.M11 = c.m11
LocatorMatrix.Matrix.M12 = c.m12
LocatorMatrix.Matrix.M13 = c.m13
LocatorMatrix.Matrix.M21 = c.m21
LocatorMatrix.Matrix.M22 = c.m22
LocatorMatrix.Matrix.M23 = c.m23
LocatorMatrix.Matrix.M31 = c.m31
LocatorMatrix.Matrix.M32 = c.m32
LocatorMatrix.Matrix.M33 = c.m33

LocatorMatrix.Matrix.M41 = c.x
LocatorMatrix.Matrix.M42 = c.y
LocatorMatrix.Matrix.M43 = c.z

P3DFile:Output()