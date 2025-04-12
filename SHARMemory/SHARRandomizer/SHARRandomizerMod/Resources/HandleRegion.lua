local Path = GetPath()
local GamePath = "/GameData/" .. Path

local P3DFile = P3D.P3DFile(GamePath)

local PowerboxTextureIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Texture, false, "powerbox.bmp")
P3DFile:RemoveChunk(PowerboxTextureIndex)
local PowerboxShaderIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Shader, false, "powerbox_m")
P3DFile:RemoveChunk(PowerboxShaderIndex)
local PowerboxShapeIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Anim_Dyna_Phys, false, "l1z6_powerbox_Shape")
P3DFile:RemoveChunk(PowerboxShapeIndex)

for index, chunk in P3DFile:GetChunksIndexed(P3D.Identifiers.Locator, true) do
	if chunk.Name:match("^PP_powerbox%d$") then
		P3DFile:RemoveChunk(index)
		print("Removed powerbox: " .. chunk.Name)
	end
end

P3DFile:Output()