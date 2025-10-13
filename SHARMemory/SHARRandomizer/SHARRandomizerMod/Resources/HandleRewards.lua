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
MFK:AddFunction("BindReward", {"schoolbu", "art\\cars\\schoolbu.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"glastruc", "art\\cars\\glastruc.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"minivanA", "art\\cars\\minivanA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"pizza", "art\\cars\\pizza.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"taxiA", "art\\cars\\taxiA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"sedanB", "art\\cars\\sedanB.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"fishtruc", "art\\cars\\fishtruc.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"garbage", "art\\cars\\garbage.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"nuctruck", "art\\cars\\nuctruck.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"votetruc", "art\\cars\\votetruc.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"ambul", "art\\cars\\ambul.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"sportsB", "art\\cars\\sportsB.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"IStruck", "art\\cars\\IStruck.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"burnsarm", "art\\cars\\burnsarm.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"pickupA", "art\\cars\\pickupA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"sportsA", "art\\cars\\sportsA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"compactA", "art\\cars\\compactA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"SUVA", "art\\cars\\SUVA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"hallo", "art\\cars\\hallo.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"coffin", "art\\cars\\coffin.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"witchcar", "art\\cars\\witchcar.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"sedanA", "art\\cars\\sedanA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"wagonA", "art\\cars\\wagonA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"icecream", "art\\cars\\icecream.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cBone", "art\\cars\\cBone.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cCellA", "art\\cars\\cCellA.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cCellB", "art\\cars\\cCellB.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cCellC", "art\\cars\\cCellC.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cCellD", "art\\cars\\cCellD.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cCube", "art\\cars\\cCube.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cMilk", "art\\cars\\cMilk.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cNonup", "art\\cars\\cNonup.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"gramR_v", "art\\cars\\gramR_v.p3d", "car", "forsale", 1, 100, "simpson"})
MFK:AddFunction("BindReward", {"cBlbart", "art\\cars\\cBlbart.p3d", "car", "forsale", 1, 100, "simpson"})


changed = true

if changed then
    MFK:Output(true)
end