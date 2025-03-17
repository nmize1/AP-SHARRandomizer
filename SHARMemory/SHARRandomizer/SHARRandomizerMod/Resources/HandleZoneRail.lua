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
end

if changed then
    P3DFile:Output()
end