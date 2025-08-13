Paths = {}
Paths.ModPath = GetModPath()
Paths.Resources = Paths.ModPath .. "/Resources/"
Paths.Lib = Paths.Resources .. "lib/"
Paths.Img = Paths.Resources .. "img/"

dofile(Paths.Lib .. "MFKLexer.lua")
dofile(GetModPath() .. "/Resources/lib/P3D2.lua")
P3D.LoadChunks(GetModPath() .. "/Resources/lib/P3DChunks")

Settings = GetSettings()

function GetGamePath(Path)
	Path = FixSlashes(Path,false,true)
	if Path:sub(1,1) ~= "/" then Path = "/GameData/"..Path end
	return Path
end

local Lato16 = P3D.P3DFile(GetModPath() .. "/Resources/lato_16.0.p3d")

FontName = "lato_16.0"
FontChunk = Lato16.Chunks[1]

local P3DFile = P3D.P3DFile("/GameData/art/l1z6.p3d")

PowerboxTexture = P3DFile:GetChunk(P3D.Identifiers.Texture, false, "powerbox.bmp")
PowerboxShader = P3DFile:GetChunk(P3D.Identifiers.Shader, false, "powerbox_m")
PowerboxShape = P3DFile:GetChunk(P3D.Identifiers.Anim_Dyna_Phys, false, "l1z6_powerbox_Shape")

local InstanceList = PowerboxShape:GetChunk(P3D.Identifiers.Instance_List)
local Scenegraph = InstanceList:GetChunk(P3D.Identifiers.Scenegraph)
local ScenegraphRoot = Scenegraph:GetChunk(P3D.Identifiers.Old_Scenegraph_Root)
local ScenegraphBranch = ScenegraphRoot:GetChunk(P3D.Identifiers.Old_Scenegraph_Branch)
local ScenegraphTransform = ScenegraphBranch:GetChunk(P3D.Identifiers.Old_Scenegraph_Transform)

Powerboxes = {}
for i=1,9 do
	local Powerbox = P3DFile:GetChunk(P3D.Identifiers.Locator, false, "PP_powerbox" .. i)
	local PowerboxTrigger = Powerbox:GetChunk(P3D.Identifiers.Trigger_Volume)
	
	local PowerboxTransform = ScenegraphTransform:GetChunk(P3D.Identifiers.Old_Scenegraph_Transform, false, "powerbox" .. i)
	
	Powerbox.Position.X = PowerboxTransform.Transform.M41
	Powerbox.Position.Y = PowerboxTransform.Transform.M42
	Powerbox.Position.Z = PowerboxTransform.Transform.M43
	
	PowerboxTrigger.HalfExtents = { X = 1.6, Y = 1.6, Z = 1.6 }
	PowerboxTrigger.Matrix.M41 = PowerboxTransform.Transform.M41
	PowerboxTrigger.Matrix.M42 = PowerboxTransform.Transform.M42
	PowerboxTrigger.Matrix.M43 = PowerboxTransform.Transform.M43
	
	Powerboxes[i] = Powerbox
end