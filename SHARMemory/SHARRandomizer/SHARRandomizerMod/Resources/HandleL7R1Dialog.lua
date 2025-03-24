local Path = GetPath()
local GamePath = "/GameData/" .. Path

if Exists(GamePath, true, false) then
	return
end

local NewGamePath = GamePath:gsub("Hom_L7", "Zm2_L7R1")
print("Redirecting \"" .. GamePath .. "\" to \"" .. NewGamePath .. "\".")
Redirect(NewGamePath)