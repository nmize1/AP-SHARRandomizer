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

changed = true

if changed then
    MFK:Output(true)
end