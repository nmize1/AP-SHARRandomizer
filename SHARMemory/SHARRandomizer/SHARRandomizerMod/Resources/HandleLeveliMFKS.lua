local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local changed = false
local Traffic = Config.TRAFFIC

local Level = Path:match("level0(%d)")
Level = tonumber(Level)

if Level == 1 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"barney", "npd", "barney_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"barney", "barney_walk1"})
    changed = true
elseif Level == 2 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"homer", "npd", "homer_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"}  ) 
    changed = true
elseif Level == 3 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"otto", "npd", "otto_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"otto", "otto_walk"})
    changed = true
elseif Level == 4 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"willie", "npd", "willie_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"willie", "willie_walk1"})
    changed = true
elseif Level == 5 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"homer", "npd", "homer_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"homer", "homer_walk1"})
    changed = true
elseif Level == 6 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"kearney", "npd", "kearney_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"kearney", "kearney_walk1"})
    changed = true
elseif Level == 7 then
    MFK:AddFunction("AddNPCCharacterBonusMission", {"zmale4", "npd", "zmale1_loc", "bm2", "exclamation", "", 0, "exclamation_shadow"} )
    MFK:AddFunction("AddBonusMissionNPCWaypoint", {"zmale4", "zmale1_walk1"})
    changed = true
else
    print("Invalid Level")
end

for Function, Index in MFK:GetFunctions("AddPurchaseCarReward", true) do
    if Function.Arguments[1] == "simpson" then
        MFK:RemoveFunction(Index)
        changed = true
    end
end

for Function, Index in MFK:GetFunctions("AddPurchaseCarNPCWaypoint", true) do
    if Function.Arguments[1] ~= "gil" then
        MFK:RemoveFunction(Index)
        changed = true
    end
end

local pedPool = {table.unpack(CharNames)}

for Function, Index in MFK:GetFunctions(nil, true) do
		local name = Function.Name:lower()
		if name == "addped" then
			MFK:RemoveFunction(Index)
		elseif name == "createpedgroup" then
			for j=1,7 do
				local randomPedIndex = math.random(#pedPool)
				local randomPed = pedPool[randomPedIndex]
				table.remove(pedPool, randomPedIndex)
				if #pedPool == 0 then
			        pedPool = {table.unpack(CharNames)}
			end
				
		    MFK:InsertFunction(Index + 1, "AddPed", {randomPed, 1})
	    end
    end
end

if(#Traffic == 35) then
    for Function, Index in MFK:GetFunctions("AddTrafficModel", true) do
	    MFK:RemoveFunction(Index)
    end
    for Function, Index in MFK:GetFunctions("CreateTrafficGroup", true) do
        local startIndex = (Level - 1) * 5 + 1
        local endIndex = startIndex + 4
	    for i = startIndex, endIndex do
		    local car = Traffic[i].Name
			
		    local args = {car, 1}
		    if math.random(3) == 1 then
			    args[3] = 1
		    end
			
		    MFK:InsertFunction(Index + 1, "AddTrafficModel", args)
	    end
    end
end

MFK:AddFunction("CreateTrafficGroup", 1)
MFK:AddFunction("AddTrafficModel", { "cFire_v", 1})
MFK:AddFunction("AddTrafficModel", { "oblit_v", 1})
MFK:AddFunction("AddTrafficModel", { "cBone", 1})
MFK:AddFunction("AddTrafficModel", { "cCola", 1})
MFK:AddFunction("AddTrafficModel", { "dune_v", 1})
MFK:AddFunction("CloseTrafficGroup")

if changed then
    MFK:Output(true)
end