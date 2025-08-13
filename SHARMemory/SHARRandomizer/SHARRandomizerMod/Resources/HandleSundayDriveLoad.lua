local Path = GetPath()
local GamePath = GetGamePath(Path)
local File = ReadFile(GamePath)
local MFK = MFKLexer.Lexer:Parse(File)

if Settings.CameraPanMode == 2 then
	P3DFiles = {}
	for func in MFK:GetFunctions("LoadP3DFile") do
		P3DFiles[GetGamePath(func.Arguments[1])] = true
	end

	local MultiControllerFunctions = {
		setmissionstartmulticontname = true,
		setanimcammulticontname = true,
	}
	MissionCamMultiControllers = {}
	MissionCamMultiControllersN = 0

	local InitPath = GamePath:gsub("sdl%.mfk", "sdi.mfk")
	local InitFile = ReadFile(InitPath)
	local InitMFK = MFKLexer.Lexer:Parse(InitFile)
	for i=1,#InitMFK.Functions do
		local func = InitMFK.Functions[i]
		local name = func.Name:lower()
		if MultiControllerFunctions[name] then
			MissionCamMultiControllers[func.Arguments[1]] = true
		end
	end
end