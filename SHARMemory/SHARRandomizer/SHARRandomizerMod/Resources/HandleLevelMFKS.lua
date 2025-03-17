local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local changed = false

MFK:AddFunction("AddBonusMission", {"bm2"})
changed = true


if changed then
    MFK:Output(true)
end