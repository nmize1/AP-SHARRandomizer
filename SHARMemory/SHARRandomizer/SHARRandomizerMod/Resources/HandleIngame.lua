if not IsTesting() and Ingame ~= nil then
	Output(Ingame)
	return
end

local Path = GetPath()
local GamePath = GetGamePath(Path)

local P3DFile = P3D.P3DFile(GamePath)

-- Begin 3D Phonebooth
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
-- End 3D Phonebooth

-- Begin AP info
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

local Hud = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "Hud.pag")
local PauseSunday = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "PauseSunday.pag")
local PauseMission = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "PauseMission.pag")
local SmallBoard = FrontendProjectChunk:GetChunk(P3D.Identifiers.Frontend_Page, false, "SmallBoard.pag")

if Hud then
	local LayerChunk = Hud:GetChunk(P3D.Identifiers.Frontend_Layer)
	assert(LayerChunk, "What the fuck your game is broken")
	
	local GroupChunk
	
	if Settings.APLog then
		GroupChunk = GroupChunk or P3D.FrontendGroupP3DChunk("Archipelago", 0, 255)
		
		P3DFile:AddChunk(FontChunk, 1)
		
		local TextStyleChunk = P3D.FrontendTextStyleResourceP3DChunk:new(FontName, 1, "fonts\\" .. FontName .. ".p3d", FontName)
		Hud:AddChunk(TextStyleChunk, 1)
		
		local APLogMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APLog", 17, {X = Settings.APLogX, Y = Settings.APLogY}, {X = Hud.Resolution.X - 40, Y = math.floor(Hud.Resolution.Y / 4)}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, FontName, 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		GroupChunk:AddChunk(APLogMultiTextChunk)
		
		local APLogTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APLog")
		APLogMultiTextChunk:AddChunk(APLogTextChunk)
	end
	
	if Settings.FillerIcons then
		GroupChunk = GroupChunk or P3D.FrontendGroupP3DChunk("Archipelago", 0, 255)
		
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

local function ModifyPause(Page)
	local LayerChunk = Page:GetChunk(P3D.Identifiers.Frontend_Layer)
	assert(LayerChunk, "What the fuck your game is broken")
	
	-- Begin Background
	if SmallBoard then
		for FrontendImageResourceChunk in SmallBoard:GetChunks(P3D.Identifiers.Frontend_Image_Resource, true) do
			Page:AddChunk(FrontendImageResourceChunk:Clone(), 1)
		end
		local BoardGroup = SmallBoard:GetChunk(P3D.Identifiers.Frontend_Layer):GetChunk(P3D.Identifiers.Frontend_Group):Clone()
		LayerChunk:AddChunk(BoardGroup)
		
		local FrameBgd = BoardGroup:GetChunk(P3D.Identifiers.Frontend_Polygon, false, "Frame_Bgd")
		FrameBgd.Points[1] = { X = 12, Y = 150, Z = 0 } -- Bottom Left
		FrameBgd.Points[2] = { X = 12, Y = 354, Z = 0 } -- Top Left
		FrameBgd.Points[3] = { X = 147, Y = 354, Z = 0 } -- Top Right
		FrameBgd.Points[4] = { X = 147, Y = 150, Z = 0 } -- Bottom Right
		
		local FrameLeft
		local FrameTop
		local FrameRight
		local FrameBottom
		for FrontendMultiSpriteChunk in BoardGroup:GetChunks(P3D.Identifiers.Frontend_Multi_Sprite) do
			FrontendMultiSpriteChunk.Position = { X = 2, Y = 143 }
			FrontendMultiSpriteChunk.Dimension = { X = 147, Y = 220 }
			if FrontendMultiSpriteChunk.Name == "Frame_Left" then
				FrameLeft = FrontendMultiSpriteChunk
			elseif FrontendMultiSpriteChunk.Name == "Frame_Top" then
				FrameTop = FrontendMultiSpriteChunk
			elseif FrontendMultiSpriteChunk.Name == "Frame_Right" then
				FrameRight = FrontendMultiSpriteChunk
			elseif FrontendMultiSpriteChunk.Name == "Frame_Bottom" then
				FrameBottom = FrontendMultiSpriteChunk
			end
		end
		
		local FrameTop2 = FrameTop:Clone()
		BoardGroup:AddChunk(FrameTop2)
		FrameTop2.Position.X = FrameTop2.Position.X - 19
		
		local FrameTop3 = FrameTop:Clone()
		BoardGroup:AddChunk(FrameTop3)
		FrameTop3.Position.X = FrameTop3.Position.X + 19
		
		local FrameBottom2 = FrameBottom:Clone()
		BoardGroup:AddChunk(FrameBottom2)
		FrameBottom2.Position.X = FrameBottom2.Position.X - 19
		
		local FrameBottom3 = FrameBottom:Clone()
		BoardGroup:AddChunk(FrameBottom3)
		FrameBottom3.Position.X = FrameBottom3.Position.X + 19
		
		local FrameLeft2 = FrameLeft:Clone()
		BoardGroup:AddChunk(FrameLeft2)
		FrameLeft2.Position.Y = FrameLeft2.Position.Y - 55
		
		local FrameLeft3 = FrameLeft:Clone()
		BoardGroup:AddChunk(FrameLeft3)
		FrameLeft3.Position.Y = FrameLeft3.Position.Y + 55
		
		local FrameLeft4 = FrameLeft:Clone()
		BoardGroup:AddChunk(FrameLeft4)
		FrameLeft4.Position.Y = FrameLeft4.Position.Y + 18
		
		FrameLeft.Position.Y = FrameLeft.Position.Y - 18
		
		local FrameRight2 = FrameRight:Clone()
		BoardGroup:AddChunk(FrameRight2)
		FrameRight2.Position.Y = FrameRight2.Position.Y - 55
		
		local FrameRight3 = FrameRight:Clone()
		BoardGroup:AddChunk(FrameRight3)
		FrameRight3.Position.Y = FrameRight3.Position.Y + 55
		
		local FrameRight4 = FrameRight:Clone()
		BoardGroup:AddChunk(FrameRight4)
		FrameRight4.Position.Y = FrameRight4.Position.Y + 18
		
		FrameRight.Position.Y = FrameRight.Position.Y - 18
	end
	-- End Background
	
	local GroupChunk = P3D.FrontendGroupP3DChunk("Archipelago", 0, 255)
	LayerChunk:AddChunk(GroupChunk)
	
	-- Begin Max Coins
	local APMaxCoinsMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APMaxCoins", 17, {X = 500, Y = 390}, {X = 200, Y = 20}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=240,G=225,B=20}, 0, 0, "font0_16", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	GroupChunk:AddChunk(APMaxCoinsMultiTextChunk)
	
	local APMaxCoinsTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APMaxCoins")
	APMaxCoinsMultiTextChunk:AddChunk(APMaxCoinsTextChunk)
	-- End Max Coins
	
	-- Begin Filler Icons
	local WrenchImageResource = P3D.FrontendImageResourceP3DChunk("Wrench", 1, "img\\Wrench.png")
	Page:AddChunk(WrenchImageResource, 1)
	
	local HnRImageResource = P3D.FrontendImageResourceP3DChunk("HnR", 1, "img\\HnR.png")
	Page:AddChunk(HnRImageResource, 1)
	
	local WrenchMultiSprite = P3D.FrontendMultiSpriteP3DChunk("Wrench", 1, {X = 15, Y = 385}, {X = 16, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, {"Wrench"})
	GroupChunk:AddChunk(WrenchMultiSprite)
	
	local HnRMultiSprite = P3D.FrontendMultiSpriteP3DChunk("HnR", 1, {X = 80, Y = 385}, {X = 16, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, {"HnR"})
	GroupChunk:AddChunk(HnRMultiSprite)
	
	local APWrenchMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APWrench", 17, {X = 50, Y = 380}, {X = 64, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font1_14", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	GroupChunk:AddChunk(APWrenchMultiTextChunk)
	
	local APWrenchTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APWrench")
	APWrenchMultiTextChunk:AddChunk(APWrenchTextChunk)
	
	local APHnRMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APHnR", 17, {X = 115, Y = 380}, {X = 64, Y = 16}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font1_14", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	GroupChunk:AddChunk(APHnRMultiTextChunk)
	
	local APHnRTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APHnR")
	APHnRMultiTextChunk:AddChunk(APHnRTextChunk)
	-- End Filler Icons
	
	-- Begin Progress
	local TextStyleChunk = P3D.FrontendTextStyleResourceP3DChunk:new("font2_12", 1, "fonts\\font2_12.p3d", "swz721mi_12")
	Page:AddChunk(TextStyleChunk, 1)
	
	local APProgressTitleMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APProgressTitle", 17, {X = 15, Y = 340}, {X = 128, Y = 300}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font1_14", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	GroupChunk:AddChunk(APProgressTitleMultiTextChunk)
	
	local APProgressTitleTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APProgressTitle")
	APProgressTitleMultiTextChunk:AddChunk(APProgressTitleTextChunk)
	
	local APProgressMultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APProgress", 17, {X = 15, Y = 320}, {X = 128, Y = 300}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, "font2_12", 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
	GroupChunk:AddChunk(APProgressMultiTextChunk)
	
	local APProgressTextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APProgress")
	APProgressMultiTextChunk:AddChunk(APProgressTextChunk)
	-- End Progress
end

if PauseSunday then
	ModifyPause(PauseSunday)
end

if PauseMission then
	ModifyPause(PauseMission)
end
-- End AP Info

local FrontendProject = P3DFile:GetChunk(P3D.Identifiers.Frontend_Project)

local GenericPrompt = FrontendProject:GetChunk(P3D.Identifiers.Frontend_Page, nil, "GenericPrompt.pag")
if GenericPrompt then
	local FrontendLayer = GenericPrompt:GetChunk(P3D.Identifiers.Frontend_Layer, nil, "Foreground")
	local FrontendMultiText = FrontendLayer:GetChunk(P3D.Identifiers.Frontend_Multi_Text, nil, "Message")
	for i=20,maxIngameMessageIdx do
		FrontendMultiText:AddChunk(P3D.FrontendStringTextBibleP3DChunk ("srr2", "INGAME_MESSAGE_" .. i))
	end
end

local PauseMissionObjective = FrontendProject:GetChunk(P3D.Identifiers.Frontend_Page, nil, "PauseMissionObjective.pag")
if PauseMissionObjective then
	local FrontendLayer = PauseMissionObjective:GetChunk(P3D.Identifiers.Frontend_Layer, nil, "Foreground")
	local FrontendGroup = FrontendLayer:GetChunk(P3D.Identifiers.Frontend_Group, nil, "MissionObjective")
	local FrontendMultiText = FrontendGroup:GetChunk(P3D.Identifiers.Frontend_Multi_Text, nil, "MissionObjective")
	for i=300,maxMissionObjectiveIdx do
		FrontendMultiText:AddChunk(P3D.FrontendStringTextBibleP3DChunk ("srr2", "MISSION_OBJECTIVE_" .. i))
	end
end

local Hud = FrontendProject:GetChunk(P3D.Identifiers.Frontend_Page, nil, "Hud.pag")
if Hud then
	local FrontendLayer = Hud:GetChunk(P3D.Identifiers.Frontend_Layer, nil, "Foreground")
	local FrontendGroup = FrontendLayer:GetChunk(P3D.Identifiers.Frontend_Group, nil, "Message")
	local FrontendMultiText = FrontendGroup:GetChunk(P3D.Identifiers.Frontend_Multi_Text, nil, "Message")
	for i=300,maxMissionObjectiveIdx do
		FrontendMultiText:AddChunk(P3D.FrontendStringTextBibleP3DChunk ("srr2", "MISSION_OBJECTIVE_" .. i))
	end
end

Ingame = tostring(P3DFile)
Output(Ingame)
