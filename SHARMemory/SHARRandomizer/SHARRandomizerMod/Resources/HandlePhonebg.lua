if PhoneBG then
    Output(PhoneBG)
    return
end

local BaseP3D = P3D.P3DFile("/GameData/art/frontend/scrooby/resource/pure3d/rewardbg.p3d")

-- Cleanup majority of unnecessary chunks
local KeepNames = {
    ["CAM_Pedestal_Camera"] = true,
    ["directionalLightShape3"] = true,
    ["directionalLightShape4"] = true,
    ["directionalLightShape2"] = true,
    ["Pedestal_LightGroup"] = true,
    ["Pedestal_Camera"] = true,
    ["Pedestal_Scenegraph"] = true,
    ["Pedestal_MasterController"] = true,
}
for index, chunk in BaseP3D:GetChunksIndexed(nil, true) do
    if not KeepNames[chunk.Name] then
        BaseP3D:RemoveChunk(index)
    end
end

-- Fix animation
local AnimationChunk = BaseP3D:GetChunk(P3D.Identifiers.Animation)
local AnimationGroupListChunk = AnimationChunk:GetChunk(P3D.Identifiers.Animation_Group_List)
local AnimationGroupChunk = AnimationGroupListChunk:GetChunk(P3D.Identifiers.Animation_Group)
for index, chunk in AnimationGroupChunk:GetChunksIndexed(P3D.Identifiers.Float_1_Channel, true) do
    if chunk.Param == "FOV\0" then
        AnimationGroupChunk:RemoveChunk(index)
        break
    end
end

-- Fix lights
local Colours = {
    directionalLightShape3 = { A = 255, R = 126, G = 126, B = 126 },
    directionalLightShape4 = { A = 255, R = 88, G = 88, B = 88 },
    directionalLightShape2 = { A = 255, R = 204, G = 204, B = 204 },
}
for chunk in BaseP3D:GetChunks(P3D.Identifiers.Light) do
    chunk.Colour = Colours[chunk.Name]
end

-- Fix camera
local CameraChunk = BaseP3D:GetChunk(P3D.Identifiers.Camera)
CameraChunk.FOV = 1.45

-- Fix Scenegraph
local ScenegraphChunk = BaseP3D:GetChunk(P3D.Identifiers.Scenegraph)
local ScenegraphRootChunk = ScenegraphChunk:GetChunk(P3D.Identifiers.Old_Scenegraph_Root)
local ScenegraphBranchChunk = ScenegraphRootChunk:GetChunk(P3D.Identifiers.Old_Scenegraph_Branch)
local ScenegraphTransformChunk = ScenegraphBranchChunk:GetChunk(P3D.Identifiers.Old_Scenegraph_Transform)
ScenegraphBranchChunk:RemoveChunk(ScenegraphTransformChunk)
ScenegraphBranchChunk:AddChunk(P3D.OldScenegraphBranchP3DChunk("workaround_DONT_DELETE_THIS"), 1)

-- Fix Multi Controller
local MultiControllerChunk = BaseP3D:GetChunk(P3D.Identifiers.Multi_Controller)
local MultiControllerTracksChunk = MultiControllerChunk:GetChunk(P3D.Identifiers.Multi_Controller_Tracks)
table.remove(MultiControllerTracksChunk.Tracks, 1)

PhoneBG = tostring(BaseP3D)
Output(PhoneBG)