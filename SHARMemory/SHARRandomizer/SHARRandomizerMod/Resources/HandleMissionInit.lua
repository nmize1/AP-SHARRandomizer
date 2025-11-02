local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local Level, Mission = Path:match("scripts[\\/]missions[\\/]level0(%d)[\\/]m(%d)i.mfk")
Level, Mission = tonumber(Level), tonumber(Mission)

if Level == 1 and Mission == 0 then
	print("Handling Tutorial")
	local MFK = MFKLexer.Lexer:New()
	MFK:AddFunction("SelectMission", "m0")

	MFK:AddFunction("AddStage", "final")
	MFK:AddFunction("AddObjective", "timer")
	MFK:AddFunction("SetDurationTime", 0)
	MFK:AddFunction("CloseObjective")
	MFK:AddFunction("CloseStage")

	MFK:AddFunction("CloseMission")
	MFK:Output()
	return
end

