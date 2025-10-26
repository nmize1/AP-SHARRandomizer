local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))
MFK:AddFunction("LoadP3DFile", "art\\frontend\\dynaload\\images\\msnicons\\object\\tshirt.p3d")

MFK:Output(true)