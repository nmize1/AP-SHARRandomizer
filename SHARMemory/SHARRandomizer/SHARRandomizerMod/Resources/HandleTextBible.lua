if TextBibleCache then -- Game loads the textbible 4 times for some reason, may as well cache it
    Output(TextBibleCache)
    return
end

local Path = GetPath()

local P3DFile = P3D.P3DFile("/GameData/" .. Path)
local BibleChunk = P3DFile:GetChunk(P3D.Identifiers.Frontend_Text_Bible)
if not BibleChunk then -- This file is fucked
    return
end

local lang
if GetGameLanguage then -- Older versions of the launcher don't have this function
    lang = GetGameLanguage()
end

local default = "If you can read this, then you are not running the Archipelago client. You can probably also access a bunch of other things you should not do."
for chunk in BibleChunk:GetChunks(P3D.Identifiers.Frontend_Language) do
	if lang == nil or chunk.Language == lang then -- If we can't find game lang, or if lang is the current game lang, add the entries
		for i=1,42 do
			chunk:AddValue("APCAR" .. i, default)
		end
	end
end

for chunk in BibleChunk:GetChunks(P3D.Identifiers.Frontend_Language) do
	if lang == nil or chunk.Language == lang then
		local Default = "APLog will show here." .. string.rep(" ", 475)
		chunk:AddValue("APLog", Default)
		
		chunk:AddValue("APWrench", "00")
		chunk:AddValue("APHnR", "00")
	end
end

TextBibleCache = tostring(P3DFile)
Output(TextBibleCache)