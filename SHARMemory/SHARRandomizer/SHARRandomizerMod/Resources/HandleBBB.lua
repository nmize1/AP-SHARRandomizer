local Path = GetPath()
local GamePath = "/GameData/" .. Path

local P3DFile = P3D.P3DFile(GamePath)

P3DFile:AddChunk(PowerboxTexture, 1)
P3DFile:AddChunk(PowerboxShader, 2)
P3DFile:AddChunk(PowerboxShape)

for i=1,#Powerboxes do
	local Powerbox = Powerboxes[i]
	P3DFile:AddChunk(Powerbox)
	print("Added powerbox: " .. Powerbox.Name)
end

P3DFile:Output()