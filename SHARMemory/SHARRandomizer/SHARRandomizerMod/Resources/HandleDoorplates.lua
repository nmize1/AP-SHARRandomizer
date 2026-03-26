local Path = GetPath()
local GamePath = "/GameData/" .. Path
local P3DFile = P3D.P3DFile(GamePath)

local SkeletonChunkOrig = P3DFile:GetChunk(P3D.Identifiers.Skeleton, true, "level1_switch")
local AnimCollChunkOrig = P3DFile:GetChunk(P3D.Identifiers.Anim_Coll, true, "level1_switch")

local xOffset = 4.4
local zOffset = 3.95

-- Create copies and set their relative locations
for i=2,7 do
	SkeletonChunk = SkeletonChunkOrig:Clone()
	AnimCollChunk = AnimCollChunkOrig:Clone()
	
	P3DFile:AddChunk(SkeletonChunk)
	P3DFile:AddChunk(AnimCollChunk)

	local CompositeDrawableProp = AnimCollChunk.Chunks[2].Chunks[2].Chunks[1]
	CompositeDrawableProp.Name = tostring(i)

	SkeletonChunk.Name = "level" .. i .. "_switch"

	local SkeletonJoint = SkeletonChunk.Chunks[1]
	SkeletonJoint.Name = SkeletonChunk.Name
	
	AnimCollChunk.Name = SkeletonChunk.Name
	local CompositeDrawable = AnimCollChunk:GetChunk(P3D.Identifiers.Composite_Drawable)
	CompositeDrawable.Name = SkeletonChunk.Name
	CompositeDrawable.SkeletonName = SkeletonChunk.Name
	local CollisionObject = AnimCollChunk:GetChunk(P3D.Identifiers.Collision_Object)
	CollisionObject.Name = SkeletonChunk.Name
	local OldFrameController = AnimCollChunk:GetChunk(P3D.Identifiers.Old_Frame_Controller)
	if OldFrameController then
		AnimCollChunk:RemoveChunk(OldFrameController)
		local MultiController = AnimCollChunk:GetChunk(P3D.Identifiers.Multi_Controller)
		local MultiControllerTracks = MultiController:GetChunk(P3D.Identifiers.Multi_Controller_Tracks)
		MultiControllerTracks.Tracks = {}
	end

	local shiftX = 0
	local shiftZ = 0
	local flip = false
	local rOffset = 2

	local transforms = {
		[2] = {x = -rOffset,             z = zOffset,   flip = true},
		[3] = {x = -xOffset,      z = 0,         flip = false},
		[4] = {x = -xOffset-rOffset,      z = zOffset,   flip = true},
		[5] = {x = -xOffset*2,    z = 0,         flip = false},
		[6] = {x = -xOffset*2-rOffset,    z = zOffset,   flip = true},
		[7] = {x = -xOffset*2.76, z = 1,	     flip = false},
	}

	local shiftX, shiftZ, flip = 0, 0, false
	local restPose = SkeletonJoint.RestPose

	if transforms[i] then
		shiftX = transforms[i].x
		shiftZ = transforms[i].z
		flip   = transforms[i].flip

		if i == 7 then
			local m11, m12, m13 = restPose.M11, restPose.M12, restPose.M13
			local m21, m22, m23 = restPose.M21, restPose.M22, restPose.M23
			local m31, m32, m33 = restPose.M31, restPose.M32, restPose.M33

		    restPose.M11 = m13
			restPose.M12 = m12
			restPose.M13 = -m11

			restPose.M21 = m23
			restPose.M22 = m22
			restPose.M23 = -m21

			restPose.M31 = m33
			restPose.M32 = m32
			restPose.M33 = -m31
		end
	end

	if flip then
		restPose.M11 = -restPose.M11
		restPose.M13 = -restPose.M13
		restPose.M31 = -restPose.M31
		restPose.M33 = -restPose.M33
	end

	restPose.M41 = restPose.M41 + shiftX
	restPose.M43 = restPose.M43 + shiftZ
end

P3DFile:Output()