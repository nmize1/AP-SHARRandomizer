local Path = GetPath()
local GamePath = GetGamePath(Path)

local SPT = SPTParser.SPTFile(GamePath)

local file = GetFileName(GamePath)

if file == "mono_v.spt" then
	sound = "generator"
elseif file == "bookb_v.spt" then
	sound = "book_fire"
else
	return
end

local class = SPTParser.Class("daSoundResourceData", sound)
class:AddMethod("AddFilename", { "sound\\carsound\\" .. sound .. ".rsd", 1.0 })
class:AddMethod("SetLooping", { true })
class:AddMethod("SetTrim", { 1.0 })
class:AddMethod("SetStreaming", { true })

SPT.Classes[#SPT.Classes + 1] = class

Output(tostring(SPT))