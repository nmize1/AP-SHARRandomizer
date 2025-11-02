local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local changed = false

for Function, Index in MFK:GetFunctions("BindReward", true) do 
    if Function.Arguments[7] == "gil" then
        Function.Arguments[7] = "simpson"
        changed = true
    elseif Function.Arguments[7] == "interior" then
        Function.Arguments[6] = "9999999"
        changed = true
    end
end

local level = 1

for i = 1, 42 do
    print("APCAR" .. i .. "Level" .. level) 
    MFK:AddFunction("BindReward", {"APCar" .. i, "art\\cars\\APCar" .. i .. ".p3d", "car", "forsale", level, 100, "gil"})
    if i % 6 == 0 then
        level = level + 1
    end
end

MFK:AddFunction("BindReward", {"rocke_v", "art\\cars\\rocke_v.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"mono_v", "art\\cars\\mono_v.p3d", "car", "forsale", 2, 250, "simpson"})
MFK:AddFunction("BindReward", {"knigh_v", "art\\cars\\knigh_v.p3d", "car", "forsale", 3, 100, "simpson"})
MFK:AddFunction("BindReward", {"atv_v", "art\\cars\\atv_v.p3d", "car", "forsale", 4, 100, "simpson"})
MFK:AddFunction("BindReward", {"oblit_v", "art\\cars\\oblit_v.p3d", "car", "forsale", 5, 100, "simpson"})
MFK:AddFunction("BindReward", {"hype_v", "art\\cars\\hype_v.p3d", "car", "forsale", 6, 100, "simpson"})
MFK:AddFunction("BindReward", {"dune_v", "art\\cars\\dune_v.p3d", "car", "forsale", 7, 100, "simpson"})

function Clamp(Value, Lo, Hi)
    if Value < Lo then return Lo end
    if Hi < Value then return Hi end
    return Value
end

local function SetCarAttributes(CarName)
	local CONFile = ReadFile("/GameData/scripts/cars/" .. CarName .. ".con")
	local CON = MFKLexer.Lexer:Parse(CONFile)
	local Speed, GasScale, TireGrip, Mass, HitPoints, NormalSteering
	for i=1,#CON.Functions do
		local func = CON.Functions[i]
		local name = func.Name:lower()

		if name == "settopspeedkmh" then
			Speed = tonumber(func.Arguments[1])
		elseif name == "setgasscale" then
			GasScale = tonumber(func.Arguments[1])
		elseif name == "settiregrip" then
			TireGrip = tonumber(func.Arguments[1])
		elseif name == "setmass" then
			Mass = tonumber(func.Arguments[1])
		elseif name == "sethitpoints" then
			HitPoints = tonumber(func.Arguments[1])
		elseif name == "setnormalsteering" then
			NormalSteering = tonumber(func.Arguments[1])
		end
	end

	local SpeedAttribute
	if Speed == nil then
		SpeedAttribute = 0
	else
		SpeedAttribute = 5 * ((Speed - 120) / 50)
		SpeedAttribute = Clamp(SpeedAttribute, 0.1, 5)
	end
	
	local AccelerationAttribute
	if GasScale == nil or TireGrip == nil then
		AccelerationAttribute = 0
	else
		AccelerationAttribute = 5 * ((GasScale * TireGrip) / 32)
		AccelerationAttribute = Clamp(AccelerationAttribute, 0.1, 5)
	end
	
	local ToughnessAttribute
	if HitPoints == nil or Mass == nil then
		ToughnessAttribute = 0
	else
		ToughnessAttribute = 5 * (((Mass / 100) + HitPoints) / 60)
		ToughnessAttribute = Clamp(ToughnessAttribute, 0.1, 5)
	end
	
	local HandlingAttribute
	if NormalSteering == nil then
		HandlingAttribute = 0
	else
		HandlingAttribute = 5 * ((NormalSteering - 50) / 50)
		HandlingAttribute = Clamp(HandlingAttribute, 0.1, 5)
	end
	MFK:AddFunction("SetCarAttributes", {CarName, SpeedAttribute, AccelerationAttribute, ToughnessAttribute, HandlingAttribute})
end

local cars = {
    "schoolbu",
    "glastruc",
    "minivanA",
    "pizza",
    "taxiA",
    "sedanB",
    "fishtruc",
    "garbage",
    "nuctruck",
    "votetruc",
    "ambul",
    "sportsB",
    "IStruck",
    "burnsarm",
    "pickupA",
    "sportsA",
    "compactA",
    "SUVA",
    "hallo",
    "coffin",
    "witchcar",
	"ship",
    "sedanA",
    "wagonA",
    "icecream",
    "cBone",
    "cCellA",
    "cCube",
    "cMilk",
    "cNonup",
    "gramR_v",
    "cBlbart",
	"tt"
}

for _, car in ipairs(cars) do
    MFK:AddFunction("BindReward", {car, "art\\cars\\" .. car .. ".p3d", "car", "forsale", 1, 100, "simpson"})
	SetCarAttributes(car)
end


changed = true

if changed then
    MFK:Output(true)
end