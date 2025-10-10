local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local Level = Path:match("level0(%d)")

local changed = false
local Path = GetPath()
local Level = Path:match("level0(%d)")
Level = tonumber(Level)

if Level == 1 then
    for Function, Index in MFK:GetFunctions("AddMission", true) do
            if Function.Arguments[1] == "m                                                                   0" then
                MFK:RemoveFunction(Index)
                changed = true
            end
    end
end

MFK:AddFunction("AddBonusMission", {"bm2"})

MFK:AddFunction("LoadP3DFile",{"art\\missions\\level0" .. Level .. "\\cards.p3d"})
changed = true


if changed then
    MFK:Output(true)
end