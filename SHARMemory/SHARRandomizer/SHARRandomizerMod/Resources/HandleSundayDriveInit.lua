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

local CarLock = LockSundayDrive[Level][Mission]

local function AddStages(idx, dummyonly)
	MFK:InsertFunction(idx, "AddStage")
	idx = idx + 1
	MFK:InsertFunction(idx, "AddObjective", "dummy")
	idx = idx + 1
	MFK:InsertFunction(idx, "CloseObjective")
	idx = idx + 1
	MFK:InsertFunction(idx, "CloseStage")
	idx = idx + 1
	if dummyonly then
		return
	end
	if CarLock then
		local indexes = MissionLock[CarLock]

		MFK:InsertFunction(idx, "AddStage")
		idx = idx + 1
		MFK:InsertFunction(idx, "AddObjective", "timer")
		idx = idx + 1
		MFK:InsertFunction(idx, "SetDurationTime", 5)
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseObjective")
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseStage")
		idx = idx + 1
	
		MFK:InsertFunction(idx, "AddStage", {"locked", "car", CarLock})
		idx = idx + 1
		print(indexes.IngameMessageIdx)
		MFK:InsertFunction(idx, "SetStageMessageIndex", indexes.IngameMessageIdx)
		idx = idx + 1
		MFK:InsertFunction(idx, "AddObjective", "timer")
		idx = idx + 1
		MFK:InsertFunction(idx, "SetDurationTime", 0)
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseObjective")
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseStage")
		idx = idx + 1
	
		MFK:InsertFunction(idx, "AddStage")
		idx = idx + 1
		print(indexes.MissionObjectiveIdx)
		MFK:InsertFunction(idx, "SetStageMessageIndex", indexes.MissionObjectiveIdx)
		idx = idx + 1
		MFK:InsertFunction(idx, "SetHUDIcon", "aplogomsn")
		idx = idx + 1
		MFK:InsertFunction(idx, "AddObjective", {"buycar", CarLock})
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseObjective")
		idx = idx + 1
		MFK:InsertFunction(idx, "CloseStage")
		idx = idx + 1
	end
end

local FirstAddStageIndex
local ResetAddStageIndex
local ResetStage = false
for i=#MFK.Functions,1,-1 do
	local func = MFK.Functions[i]
	local name = func.Name:lower()
	if name == "addstage" then
		FirstAddStageIndex = i
		if ResetStage then
			ResetAddStageIndex = i
		end
	elseif name == "reset_to_here" then
		ResetStage = true
		table.remove(MFK.Functions, i)
	end
end

if ResetAddStageIndex then
	AddStages(ResetAddStageIndex, false)
	AddStages(FirstAddStageIndex, true)
else
	AddStages(FirstAddStageIndex, false)
end


if Settings.RemoveInitialWalk then
	local SetInitialWalk, SetInitialWalkIndex = MFK:GetFunction("SetInitialWalk")
	if SetInitialWalk then
		local SetMissionResetPlayerOutCar = MFK:GetFunction("SetMissionResetPlayerOutCar")
		if SetMissionResetPlayerOutCar then
			SetMissionResetPlayerOutCar.Arguments[1] = SetInitialWalk.Arguments[1]
		end
		MFK:RemoveFunction(SetInitialWalkIndex)
	end
end

if Settings.CameraPanMode == 3 then
	local ToRemove = {
		["setmissionstartcameraname"] = true,
		["setmissionstartmulticontname"] = true,
		["setanimatedcameraname"] = true,
		["setanimcammulticontname"] = true,
	}
	
	for i=#MFK.Functions,1,-1 do
		local func = MFK.Functions[i]
		local name = func.Name:lower()
		if ToRemove[name] then
			table.remove(MFK.Functions, i)
		end
	end
end

if Level == 7 and Mission == 5 then
	print("Moving spawn point.")
	for Function, Index in MFK:GetFunctions("SetMissionResetPlayerOutCar", true) do
		if Function.Arguments[1] == "m5_homer_start" then
			Function.Arguments[1] = "m5_carstart"
		end
	end
end

MFK:Output(true)
