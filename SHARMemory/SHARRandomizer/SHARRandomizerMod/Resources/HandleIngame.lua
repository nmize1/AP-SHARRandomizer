if Ingame ~= nil then
	Output(Ingame)
	return
end

local Path = GetPath()
local GamePath = GetGamePath(Path)

local P3DFile = P3D.P3DFile(GamePath)

local FrontendProjectIndex, FrontendProjectChunk = P3DFile:GetChunkIndexed(P3D.Identifiers.Frontend_Project)
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

local Hud = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "Hud.pag")
local PauseSunday = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "PauseSunday.pag")

if Hud then
	local LayerChunk = Hud:GetChunk(P3D.Identifiers.Frontend_Layer)
	assert(LayerChunk, "What the fuck your game is broken")
	
	local GroupChunk
	
	if GetSetting("APLog") then
		GroupChunk = GroupChunk or P3D.FrontendGroupP3DChunk("Archipelago", 0, 255)
		
		P3DFile:AddChunk(FontChunk, 1)
		
		local TextStyleChunk = P3D.FrontendTextStyleResourceP3DChunk:new(FontName, 1, "fonts\\" .. FontName .. ".p3d", FontName)
		Hud:AddChunk(TextStyleChunk, 1)
		
		local APLogMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APLog", 17, {X = GetSetting("APLogX"), Y = GetSetting("APLogY")}, {X = Hud.Resolution.X - 40, Y = math.floor(Hud.Resolution.Y / 4)}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, FontName, 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		GroupChunk:AddChunk(APLogMultiTextChunk)
		
		local APLogTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APLog")
		APLogMultiTextChunk:AddChunk(APLogTextChunk)
	end
	
	if GetSetting("FillerIcons") then
		GroupChunk = GroupChunk or P3D.FrontendGroupP3DChunk("Archipelago", 0, 255)
		
		local WrenchSprite = P3D.SpriteP3DChunk("Wrench.png", 640, 480, "", 32, 32, 1)
		local WrenchImage = P3D.ImageP3DChunk("Wrench.png", 14000, 32, 32, 32, 0, 1, P3D.ImageP3DChunk.Formats.PNG)
		WrenchSprite:AddChunk(WrenchImage)
		local WrenchImageData = P3D.ImageDataP3DChunk(ReadFile(Paths.Img .. "Wrench.png"))
		WrenchImage:AddChunk(WrenchImageData)
		P3DFile:AddChunk(WrenchSprite, 1)
		
		local HnRSprite = P3D.SpriteP3DChunk("HnR.png", 640, 480, "", 32, 32, 1)
		local HnRImage = P3D.ImageP3DChunk("HnR.png", 14000, 32, 32, 32, 0, 1, P3D.ImageP3DChunk.Formats.PNG)
		HnRSprite:AddChunk(HnRImage)
		local HnRImageData = P3D.ImageDataP3DChunk(ReadFile(Paths.Img .. "HnR.png"))
		HnRImage:AddChunk(HnRImageData)
		P3DFile:AddChunk(HnRSprite, 1)
		
		local WrenchImageResource = P3D.FrontendImageResourceP3DChunk("Wrench", 1, "img\\Wrench.png")
		Hud:AddChunk(WrenchImageResource, 1)
		
		local HnRImageResource = P3D.FrontendImageResourceP3DChunk("HnR", 1, "img\\HnR.png")
		Hud:AddChunk(HnRImageResource, 1)
		
		local WrenchMultiSprite = P3D.FrontendMultiSpriteP3DChunk("Wrench", 1, {X = 462, Y = 25}, {X = 16, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, {"Wrench"})
		GroupChunk:AddChunk(WrenchMultiSprite)
		
		local HnRMultiSprite = P3D.FrontendMultiSpriteP3DChunk("HnR", 1, {X = 537, Y = 25}, {X = 16, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, {"HnR"})
		GroupChunk:AddChunk(HnRMultiSprite)
		
		local APWrenchMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APWrench", 17, {X = 497, Y = 20}, {X = 64, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font1_14", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		GroupChunk:AddChunk(APWrenchMultiTextChunk)
		
		local APWrenchTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APWrench")
		APWrenchMultiTextChunk:AddChunk(APWrenchTextChunk)
		
		local APHnRMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APHnR", 17, {X = 572, Y = 20}, {X = 64, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font1_14", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		GroupChunk:AddChunk(APHnRMultiTextChunk)
		
		local APHnRTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APHnR")
		APHnRMultiTextChunk:AddChunk(APHnRTextChunk)
	end
	
	if GroupChunk then
		LayerChunk:AddChunk(GroupChunk)
	end
end

if PauseSunday then
	local LayerChunk = PauseSunday:GetChunk(P3D.Identifiers.Frontend_Layer)
	assert(LayerChunk, "What the fuck your game is broken")
	
	local APMaxCoinsMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APMaxCoins", 17, {X = 500, Y = 390}, {X = 200, Y = 20}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=240,G=225,B=20}, 0, 0, "font0_16", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	LayerChunk:AddChunk(APMaxCoinsMultiTextChunk)
	
	local APMaxCoinsTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APMaxCoins")
	APMaxCoinsMultiTextChunk:AddChunk(APMaxCoinsTextChunk)
end

Ingame = tostring(P3DFile)
Output(Ingame)
