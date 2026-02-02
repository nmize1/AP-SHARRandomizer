local Path = GetPath()
local GamePath = "/GameData/" .. Path

local P3DFile = P3D.P3DFile(GamePath)

local SkeletonChunk = P3DFile:GetChunk(P3D.Identifiers.Skeleton, true, "level1_switch")
local AnimCollChunk = P3DFile:GetChunk(P3D.Identifiers.Anim_Coll, true, "level1_switch")
local LocatorChunk = P3DFile:GetChunk(P3D.Identifiers.Locator, true, "Level1")

local Offset = 1.5
for i=2,7 do
	SkeletonChunk = SkeletonChunk:Clone()
	AnimCollChunk = AnimCollChunk:Clone()
	LocatorChunk = LocatorChunk:Clone()
	
	P3DFile:AddChunk(SkeletonChunk)
	P3DFile:AddChunk(AnimCollChunk)
	P3DFile:AddChunk(LocatorChunk)
	
	SkeletonChunk.Name = "level" .. i .. "_switch"
	local SkeletonJoint = SkeletonChunk.Chunks[1]
	SkeletonJoint.Name = SkeletonChunk.Name
	SkeletonJoint.RestPose.M43 = SkeletonJoint.RestPose.M43 + Offset
	
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
	
	LocatorChunk.Name = "Level" .. i
	LocatorChunk.JointName = SkeletonChunk.Name
	LocatorChunk.ObjectName = "DB_level" .. i
	LocatorChunk.Position.Z = LocatorChunk.Position.Z + Offset
	local TriggerVolume = LocatorChunk:GetChunk(P3D.Identifiers.Trigger_Volume)
	TriggerVolume.Name = "Level" .. i .. "Trigger"
	TriggerVolume.Matrix.M43 = TriggerVolume.Matrix.M43 + Offset
	local LocatorMatrix = LocatorChunk:GetChunk(P3D.Identifiers.Locator_Matrix)
	LocatorMatrix.Matrix.M43 = LocatorMatrix.Matrix.M43 + Offset
end

P3DFile:Output()