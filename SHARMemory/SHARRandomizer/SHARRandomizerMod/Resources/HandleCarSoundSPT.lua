local Path = GetPath()
local GamePath = GetGamePath(Path)

local SPT = SPTParser.SPTFile(GamePath)


Output(tostring(SPT))