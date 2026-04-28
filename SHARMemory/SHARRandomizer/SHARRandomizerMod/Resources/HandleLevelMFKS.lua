local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local Level = Path:match("level0(%d)")
local Traffic = Config.TRAFFIC

local Path = GetPath()
local Level = GetCurrentLevel()


MFK:AddFunction("AddBonusMission", {"bm2"})

MFK:AddFunction("LoadP3DFile",{"art\\missions\\level0" .. Level .. "\\cards.p3d"})
MFK:AddFunction("LoadP3DFile",{"art\\returndoorbell.p3d"})

if(#Config.TRAFFIC == 35) then
	print("Adding traffic cars")
	local startIndex = (Level - 1) * 5 + 1
	local endIndex = startIndex + 4
	for i = startIndex, endIndex do
		--print("art\\cars\\" .. Traffic[i].Name .. ".p3d")
		MFK:AddFunction("LoadP3DFile",{"art\\cars\\" .. Traffic[i].Name .. ".p3d"})
	end
end

MFK:AddFunction("LoadP3DFile",{"art\\cars\\cFire_v.p3d"})
MFK:AddFunction("LoadP3DFile",{"art\\cars\\oblit_v.p3d"})
MFK:AddFunction("LoadP3DFile",{"art\\cars\\cBone.p3d"})
MFK:AddFunction("LoadP3DFile",{"art\\cars\\cCola.p3d"})
MFK:AddFunction("LoadP3DFile",{"art\\cars\\dune_v.p3d"})

MFK:AddFunction("LoadP3DFile", "art\\frontend\\dynaload\\images\\msnicons\\object\\ApLogoMSN.p3d")

if(Level == 1) then
	MFK:AddFunction("AddBonusMission", {"ismovie"})
	MFK:AddFunction("GagBegin",{"gag_ismv.p3d"})
	MFK:AddFunction("GagSetPosition",{-6, 1, -3.5})
	MFK:AddFunction("GagSetRandom",{0})
	MFK:AddFunction("GagSetCycle",{"reset"})
	MFK:AddFunction("GagCheckMovie",{"teen", "homer", "fmv8.rmv", "aztec"})
	MFK:AddFunction("GagSetTrigger", {"action", -6, 1, -3.5, 2.0})
	MFK:AddFunction("GagSetSparkle", {0})
	MFK:AddFunction("GagEnd", {})


MFK:Output(true)