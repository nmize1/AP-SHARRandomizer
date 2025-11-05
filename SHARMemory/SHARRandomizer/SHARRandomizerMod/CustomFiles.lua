Paths = {}
Paths.ModPath = GetModPath()
Paths.Resources = Paths.ModPath .. "/Resources/"
Paths.Lib = Paths.Resources .. "lib/"
Paths.Img = Paths.Resources .. "img/"
dofile(Paths.Lib .. "IniParser.lua")
dofile(Paths.Lib .. "SPTParser.lua")
dofile(Paths.Lib .. "MFKLexer.lua")
dofile(Paths.Lib .. "P3D2.lua")
P3D.LoadChunks(Paths.Lib .. "P3DChunks")
dofile(Paths.Lib .. "Utils.lua")

Settings = GetSettings()

function GetGamePath(Path)
	Path = FixSlashes(Path,false,true)
	if Path:sub(1,1) ~= "/" then Path = "/GameData/"..Path end
	return Path
end

local ConfigPath = "/UserData/SavedGames/SHAR.ini"

while true do
	while not Exists(ConfigPath, true, false) do
		--[[ 1.27 version:
		if Dialog(DialogIcon.Error, DialogButtons.RetryClose, "Please download the patch file from the room and follow the setup guide to place it in the right location.", "`SHAR.ini` config file not found") == DialogResult.Close then
			os.exit()
		end
		]]
		Alert("`SHAR.ini` config file not found.\nPlease download the patch file from the room and follow the setup guide to place it in the right location.")
	end

	Config = IniParser(ConfigPath)

	if not Config.IDENTIFIER then
		Alert("Outdated SHAR.ini detected.\nPlease download the patch file from the room and follow the setup guide to place it in the right location.")

	elseif not (Config.CARD or not #Config.CARD == 49) then
		Alert("Missing card entries in SHAR.ini.\nPlease redownload the patch file from the room and follow the setup guide to place it in the right location.")

	elseif not Config.TRAFFIC or not (#Config.TRAFFIC == 1 or #Config.TRAFFIC == 35) then
		Alert("Missing traffic entries in SHAR.ini.\nPlease redownload the patch file from the room and follow the setup guide to place it in the right location.")

	elseif not Config.MISSIONLOCK or (#Config.MISSIONLOCK == 1 and not Config.MISSIONLOCK[1].Mission == 0) then
		Alert("Missing mission lock entries in SHAR.ini.\nPlease redownload the patch file from the room and follow the setup guide to place it in the right location.")

	else
		break
	end
end

local Lato16 = P3D.P3DFile(GetModPath() .. "/Resources/lato_16.0.p3d")

FontName = "lato_16.0"
FontChunk = Lato16.Chunks[1]

local P3DFile = P3D.P3DFile("/GameData/art/l1z6.p3d")

PowerboxTexture = P3DFile:GetChunk(P3D.Identifiers.Texture, false, "powerbox.bmp")
PowerboxShader = P3DFile:GetChunk(P3D.Identifiers.Shader, false, "powerbox_m")
PowerboxShape = P3DFile:GetChunk(P3D.Identifiers.Anim_Dyna_Phys, false, "l1z6_powerbox_Shape")

local InstanceList = PowerboxShape:GetChunk(P3D.Identifiers.Instance_List)
local Scenegraph = InstanceList:GetChunk(P3D.Identifiers.Scenegraph)
local ScenegraphRoot = Scenegraph:GetChunk(P3D.Identifiers.Old_Scenegraph_Root)
local ScenegraphBranch = ScenegraphRoot:GetChunk(P3D.Identifiers.Old_Scenegraph_Branch)
local ScenegraphTransform = ScenegraphBranch:GetChunk(P3D.Identifiers.Old_Scenegraph_Transform)

Powerboxes = {}
for i=1,9 do
	local Powerbox = P3DFile:GetChunk(P3D.Identifiers.Locator, false, "PP_powerbox" .. i)
	local PowerboxTrigger = Powerbox:GetChunk(P3D.Identifiers.Trigger_Volume)
	
	local PowerboxTransform = ScenegraphTransform:GetChunk(P3D.Identifiers.Old_Scenegraph_Transform, false, "powerbox" .. i)
	
	Powerbox.Position.X = PowerboxTransform.Transform.M41
	Powerbox.Position.Y = PowerboxTransform.Transform.M42
	Powerbox.Position.Z = PowerboxTransform.Transform.M43
	
	PowerboxTrigger.HalfExtents = { X = 1.6, Y = 1.6, Z = 1.6 }
	PowerboxTrigger.Matrix.M41 = PowerboxTransform.Transform.M41
	PowerboxTrigger.Matrix.M42 = PowerboxTransform.Transform.M42
	PowerboxTrigger.Matrix.M43 = PowerboxTransform.Transform.M43
	
	Powerboxes[i] = Powerbox
end


CharP3DFiles = {}
CharNames = {}
CharCount = 0
GetFilesInDirectory("/GameData/art/chars", CharP3DFiles, ".p3d")

local ExcludedChars = {["npd_m"]=true,["ndr_m"]=true,["nps_m"]=true}
for i=#CharP3DFiles,1,-1 do
	local filePath = CharP3DFiles[i]
	local fileName = RemoveFileExtension(GetFileName(filePath))
	if fileName:sub(-2) ~= "_m" or ExcludedChars[fileName] then
		table.remove(CharP3DFiles, i)
	else
		local P3DFile = P3D.P3DFile(filePath)
		local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
		if not CompositeDrawable then
			table.remove(CharP3DFiles, i)
		else
			CharCount = CharCount + 1
			table.insert(CharNames, 1, CompositeDrawable.Name:sub(1, -3))
		end
	end
end
print(string.format("Loaded %i characters", CharCount))

CarP3DFiles = {}
CarNames = {}
CarCount = 0
GetFilesInDirectory("/GameData/art/cars", CarP3DFiles, ".p3d")

local ExcludedCars = {["huskA"]=true, ["common"]=true}
for i=#CarP3DFiles,1,-1 do
	local filePath = CarP3DFiles[i]
	local fileName = RemoveFileExtension(GetFileName(filePath))
	if ExcludedCars[fileName] then
		table.remove(CarP3DFiles, i)
	else
		local P3DFile = P3D.P3DFile(filePath)
		local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
		if not CompositeDrawable then
			table.remove(CarP3DFiles, i)
		else
			CarCount = CarCount + 1
			table.insert(CarNames, 1, CompositeDrawable.Name)
		end
	end
end
print(string.format("Loaded %i cars", CarCount))

MissionLock = {}
LockSundayDrive = {}

for i=1,7 do
	LockSundayDrive[i] = {}
end

IngameMessageIdx = 20
MissionObjectiveIdx = 300

if not MissionLock["fakecar"] then
	MissionLock["fakecar"] = {
		IngameMessageIdx = IngameMessageIdx,
		MissionObjectiveIdx = MissionObjectiveIdx,
	}

	LockSundayDrive[1][0] = "fakecar"
end

if Config.MISSIONLOCK and Config.MISSIONLOCK[1].Mission ~= 0 then
	for i, lock in pairs(Config.MISSIONLOCK) do
		carName = lock.Car
		if not MissionLock[carName] then
			IngameMessageIdx = IngameMessageIdx + 1
			MissionObjectiveIdx = MissionObjectiveIdx + 1
			MissionLock[carName] = {
				IngameMessageIdx = IngameMessageIdx,
				MissionObjectiveIdx = MissionObjectiveIdx,
			}
		end
		local missionIndex = lock.Mission
		local level = math.floor((missionIndex - 1) / 7) + 1
		local mission = ((missionIndex - 1) % 7) + 1

		LockSundayDrive[level][mission] = carName
	end
end



for k, v in pairs(MissionLock) do
	print(k, v)
end
