if Ingame ~= nil then
	Output(Ingame)
	return
end

local Path = GetPath()
local GamePath = GetGamePath(Path)

local P3DFile = P3D.P3DFile(GamePath)


local FrontendProjectChunk = P3DFile:GetChunk(P3D.Identifiers.Frontend_Project)
local PhoneboothPageChunk = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "PhoneBooth.pag")

local ForegroundLayerIndex, ForegroundLayerChunk = PhoneboothPageChunk:GetChunkIndexed(P3D.Identifiers.Frontend_Layer, false, "Foreground")

PhoneboothPageChunk:AddChunk(P3D.FrontendPure3DResourceP3DChunk("3Dmodel", 1, "pure3d\\_stubs\\dummy.p3d", "dummy", "Pedestal_Camera", "", ""), ForegroundLayerIndex)
PhoneboothPageChunk:AddChunk(P3D.FrontendPure3DResourceP3DChunk("phonebg", 1, "pure3d\\_stubs\\dummy.p3d", "dummy", "Pedestal_Camera", "", ""), ForegroundLayerIndex + 1)
PhoneboothPageChunk:AddChunk(P3D.FrontendPure3DResourceP3DChunk("rewardfg", 1, "pure3d\\rewardfg.p3d", "PurchaseScene", "Pedestal_Camera", "", ""), ForegroundLayerIndex + 2)

local GroupChunk = P3D.FrontendGroupP3DChunk("3DModel", 0, 255)
ForegroundLayerChunk:AddChunk(GroupChunk)

GroupChunk:AddChunk(P3D.FrontendPure3DObjectP3DChunk("RewardBG", 1, {X = 29, Y = 145}, {X = 400, Y = 300}, {X = P3D.FrontendPure3DObjectP3DChunk.Justifications.Left, Y = P3D.FrontendPure3DObjectP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "phonebg"))
GroupChunk:AddChunk(P3D.FrontendPure3DObjectP3DChunk("RewardFG", 1, {X = 29, Y = 145}, {X = 400, Y = 300}, {X = P3D.FrontendPure3DObjectP3DChunk.Justifications.Left, Y = P3D.FrontendPure3DObjectP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "rewardfg"))
GroupChunk:AddChunk(P3D.FrontendPure3DObjectP3DChunk("PreviewWindow", 1, {X = 29, Y = 145}, {X = 400, Y = 300}, {X = P3D.FrontendPure3DObjectP3DChunk.Justifications.Left, Y = P3D.FrontendPure3DObjectP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "3Dmodel"))

local Hud
local PauseChunk
for chunk in FrontendProjectChunk:GetChunks(P3D.Identifiers.Frontend_Page) do
	if chunk.Name == "Hud.pag" then
		Hud = chunk
		break
	end
end

if GetSetting("APLog") then
	if Hud then
		P3DFile:AddChunk(FontChunk, 1)
	
		local TextStyleChunk = P3D.FrontendTextStyleResourceP3DChunk:new(FontName, 1, "fonts\\" .. FontName .. ".p3d", FontName)
		Hud:AddChunk(TextStyleChunk, 1)
	
		local LayerChunk = Hud:GetChunk(P3D.Identifiers.Frontend_Layer)
		local MultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APLog", 17, {X = 0, Y = 80}, {X = Hud.Resolution.X - 40, Y = math.floor(Hud.Resolution.Y / 4)}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, FontName, 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		local TextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APLog")
		MultiTextChunk:AddChunk(TextChunk)
		LayerChunk:AddChunk(MultiTextChunk)
	end
end

Ingame = tostring(P3DFile)
Output(Ingame)
