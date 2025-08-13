if Frontend ~= nil then
	Output(Frontend)
	return
end

local Path = GetPath()
local GamePath = "/GameData/" .. Path

local P3DFile = P3D.P3DFile(GamePath)

local ProjectChunk = P3DFile:GetChunk(P3D.Identifiers.Frontend_Project)
local PageChunk
for chunk in ProjectChunk:GetChunks(P3D.Identifiers.Frontend_Page) do
	if chunk.Name == "TVFrame.pag" then
		PageChunk = chunk
		break
	end
end

if Settings.APLog then
	if PageChunk then
		P3DFile:AddChunk(FontChunk, 1)
	
		local TextStyleChunk = P3D.FrontendTextStyleResourceP3DChunk:new(FontName, 1, "fonts\\" .. FontName .. ".p3d", FontName)
		PageChunk:AddChunk(TextStyleChunk, 1)
	
		local LayerChunk = PageChunk:GetChunk(P3D.Identifiers.Frontend_Layer)
		local MultiTextChunk = P3D.FrontendMultiTextP3DChunk:new("APLog", 17, {X = 20, Y = 80}, {X = PageChunk.Resolution.X - 40, Y = math.floor(PageChunk.Resolution.Y / 4)}, {X = P3D.FrontendMultiTextP3DChunk.Justifications.Left, Y = P3D.FrontendMultiTextP3DChunk.Justifications.Top}, {A=255,R=255,G=255,B=255}, 0, 0, FontName, 1, {A=192,R=0,G=0,B=0}, {X = 2, Y = -2}, 0)
		local TextChunk = P3D.FrontendStringTextBibleP3DChunk:new("srr2", "APLog")
		MultiTextChunk:AddChunk(TextChunk)
		LayerChunk:AddChunk(MultiTextChunk)
	
		Frontend = tostring(P3DFile)
		Output(Frontend)
	end
end