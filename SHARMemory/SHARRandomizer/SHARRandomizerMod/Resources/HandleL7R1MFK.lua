local Path = GetPath()
local GamePath = "/GameData/" .. Path

local MFK = MFKLexer.Lexer:Parse(ReadFile(GamePath))

local SetDialogueInfo = MFK:GetFunction("SetDialogueInfo", true)
SetDialogueInfo.Arguments[1] = "homer"
SetDialogueInfo.Arguments[2] = "npd"

local AddNPC = MFK:GetFunction("AddNPC", true)
AddNPC.Arguments[1] = "npd"

MFK:Output(true)