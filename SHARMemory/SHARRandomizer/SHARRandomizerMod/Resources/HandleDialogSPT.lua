local Path = GetPath()
local GamePath = "/GameData/" .. Path

print("Handling dialog SPT: " .. Path)

local SPT = SPTParser.SPTFile(GamePath)

for i=0,7 do
    local class = SPTParser.Class("daSoundResourceData", "W_Doorbell_Lvl" .. i .. "_Archipelago")
    class:AddMethod("AddFilename", { "Level" .. i .. "/W_Doorbell_Lvl" .. i .. "_Archipelago.rsd", 1.0 })
    class:AddMethod("SetStreaming", { true })
    SPT.Classes[#SPT.Classes + 1] = class
end

local res = tostring(SPT)
res = res:gsub("Zm2_L7R1", "Hom_L7")

Output(tostring(res))