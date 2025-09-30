local Path = GetPath()
local GamePath = "/GameData/" .. Path

local P3DFile = P3D.P3DFile(GamePath)

local changed = false

for index, chunk in P3DFile:GetChunksIndexed(P3D.Identifiers.Locator, true) do
	if chunk.Type == 3 and chunk.FreeCar then
		print("Removing " .. chunk.Name .. " from " .. Path)
		P3DFile:RemoveChunk(index)
		changed = true
	end

	if chunk.Type == 9 and chunk.ActionName == "CollectorCard" then
		P3DFile:RemoveChunk(index)
		print("Removing " .. chunk.Name .. " from " .. Path)
	end
end

local level, zone = Path:match("l(%d)z(%d)%.p3d$")
if (level == "1" or level == "4") and zone == "6" then
	local PowerboxTextureIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Texture, false, "powerbox.bmp")
	if PowerboxTextureIndex then
		P3DFile:RemoveChunk(PowerboxTextureIndex)
		changed = true
	end
	
	local PowerboxShaderIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Shader, false, "powerbox_m")
	if PowerboxShaderIndex then
		P3DFile:RemoveChunk(PowerboxShaderIndex)
		changed = true
	end
	
	local PowerboxShapeIndex = P3DFile:GetChunkIndexed(P3D.Identifiers.Anim_Dyna_Phys, false, "l1z6_powerbox_Shape")
	if PowerboxShapeIndex then
		P3DFile:RemoveChunk(PowerboxShapeIndex)
		changed = true
	end

	for index, chunk in P3DFile:GetChunksIndexed(P3D.Identifiers.Locator, true) do
		if chunk.Name:match("^PP_powerbox%d$") then
			P3DFile:RemoveChunk(index)
			changed = true
			print("Removed powerbox: " .. chunk.Name)
		end
	end
end

if changed then
	P3DFile:Output()
end