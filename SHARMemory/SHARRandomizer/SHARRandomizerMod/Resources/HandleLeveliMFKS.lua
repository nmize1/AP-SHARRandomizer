local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local changed = false

local Path = GetPath()
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

if changed then
    MFK:Output(true)
end