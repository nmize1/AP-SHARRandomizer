local Path = GetPath()
local GamePath = GetGamePath(Path)
local CON = MFKLexer.Lexer:Parse(ReadFile(GamePath))

CON:AddFunction("SetCharactersVisible", 1)
CON:AddFunction("SetDriver", "none")
CON:AddFunction("SetHasDoors", 0)
CON:AddFunction("SetIrisTransition", 1)
CON:AddFunction("SetShadowAdjustments", {-0.65, -0.45, -0.6, 0.5, -0.6, 0.15, -0.6, 0.7})

CON:Output(true)