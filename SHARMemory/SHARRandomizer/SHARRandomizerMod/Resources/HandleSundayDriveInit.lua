local Path = GetPath()

local Level, Mission = Path:match("scripts[\\/]missions[\\/]level0(%d)[\\/]m(%d)sdi.mfk")
Level, Mission = tonumber(Level), tonumber(Mission)

if Mission < 1 or Mission > 7 then
	-- Don't mess with tutorial or cutscenes
	print("Not adding dummy to tutorial/cutscene.")
	return
end

print(string.format("Adding dummy to L%dM%d", Level, Mission))

local GamePath = GetGamePath(Path)
local File = ReadFile(GamePath)
local MFK = MFKLexer.Lexer:Parse(File)

local LastStageIndex
for i=1,#MFK.Functions do
	local func = MFK.Functions[i]
	local name = func.Name:lower()
	if name == "addstage" then
		if not LastStageIndex then
			MFK:InsertFunction(i, "CloseStage")
			MFK:InsertFunction(i, "CloseObjective")
			MFK:InsertFunction(i, "AddObjective", "dummy")
			MFK:InsertFunction(i, "AddStage")
		end
		LastStageIndex = i
	elseif name == "reset_to_here" then
		table.remove(MFK.Functions, i)
		MFK:InsertFunction(LastStageIndex, "CloseStage")
		MFK:InsertFunction(LastStageIndex, "CloseObjective")
		MFK:InsertFunction(LastStageIndex, "AddObjective", "dummy")
		MFK:InsertFunction(LastStageIndex, "RESET_TO_HERE")
		MFK:InsertFunction(LastStageIndex, "AddStage")
		break
	end
end

MFK:Output(true)
