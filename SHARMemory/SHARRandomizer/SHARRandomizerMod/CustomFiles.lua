Paths = {}
Paths.ModPath = GetModPath()
Paths.Resources = Paths.ModPath .. "/Resources/"
Paths.Lib = Paths.Resources .. "lib/"

dofile(Paths.Lib .. "MFKLexer.lua")
dofile(GetModPath() .. "/Resources/lib/P3D2.lua")
P3D.LoadChunks(GetModPath() .. "/Resources/lib/P3DChunks")

function GetGamePath(Path)
	Path = FixSlashes(Path,false,true)
	if Path:sub(1,1) ~= "/" then Path = "/GameData/"..Path end
	return Path
end

