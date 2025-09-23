local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local Level = Path:match("level0(%d)")

local changed = false

MFK:AddFunction("AddBonusMission", {"bm2"})

MFK:AddFunction("LoadP3DFile",{"art\\missions\\level0" .. Level .. "\\cards.p3d"})
changed = true


if changed then
    MFK:Output(true)
end