if not P3DFiles then
	return
end

local Path = GetPath()
local GamePath = GetGamePath(Path)

if not P3DFiles[GamePath] then
	return
end
P3DFiles[GamePath] = nil

local P3DFile = P3D.P3DFile(GamePath)
local Changed = false

if MissionCamMultiControllers then
	for MissionCamMultiControllers in pairs(MissionCamMultiControllers) do
		local MultiController = P3DFile:GetChunk(P3D.Identifiers.Multi_Controller, true, MissionCamMultiController)
		if MultiController then
			MultiController.Framerate = MultiController.Framerate * Settings.FastPanInScale
			
			local MultiControllerTracks = MultiController:GetChunk(P3D.Identifiers.Multi_Controller_Tracks)
			if MultiControllerTracks then
				for i=1,#MultiControllerTracks.Tracks do
					local Animation = P3DFile:GetChunk(P3D.Identifiers.Animation, false, MultiControllerTracks.Tracks[i].Name)
					if Animation then
						Animation.FrameRate = Animation.FrameRate * Settings.FastPanInScale
					end
				end
			end
			
			Changed = true
		end
	end
end

if Changed then
	P3DFile:Output()
end