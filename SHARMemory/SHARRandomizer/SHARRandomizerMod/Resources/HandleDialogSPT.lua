local Path = GetPath()
local GamePath = "/GameData/" .. Path

print("Handling dialog SPT: " .. Path)

local contents = ReadFile(GamePath)

contents = contents:gsub("Zm2_L7R1", "Hom_L7")

Output(contents)