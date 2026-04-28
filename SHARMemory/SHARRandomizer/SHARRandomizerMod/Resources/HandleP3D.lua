local Path = GetPath()
local GamePath = GetGamePath(Path)

local Match = Path:lower()
if Match:match("^art[\\/]doorbells%.p3d$") then
    dofile(GetModPath() .. "/Resources/HandleDoorbells.lua")
    return
end

if Match:match("^art[\\/]doorplates%.p3d$") then
    dofile(GetModPath() .. "/Resources/HandleDoorplates.lua")
    return
end

if Match:match("^art[\\/]returndoorbell%.p3d$") then
    dofile(GetModPath() .. "/Resources/HandleReturnDoorbell.lua")
    return
end

if Match:match("^art[\\/]missions[\\/]level0[1-7][\\/]level%.p3d$") then
	dofile(GetModPath() .. "/Resources/HandleLevelP3D.lua")
	return
end

if not P3DFiles then
	return
end


if not P3DFiles[GamePath] then
	return
end
P3DFiles[GamePath] = nil

local P3DFile
local Changed = false

if MissionCamMultiControllers then
	P3DFile = P3DFile or P3D.P3DFile(GamePath)
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

if P3DFile and Changed then
	P3DFile:Output()
end