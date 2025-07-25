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

local TranslationMap = {
	["E"] = {
		["SKINN_V"] = "Skinner's Sedan",
		["MOE_V"] = "Moe's Sedan",
	},
	["F"] = {
		["SKINN_V"] = "La Berline de Skinner",
		["MOE_V"] = "La Berline de Moe",
	},
	["G"] = {
		["SKINN_V"] = "Skinners Wagen",
		["MOE_V"] = "Moes Wagen",
	},
	["S"] = {
		["SKINN_V"] = "El Sedán de Skinner",
		["MOE_V"] = "El Sedán de Moe",
	},
}

for chunk in BibleChunk:GetChunks(P3D.Identifiers.Frontend_Language) do
	if lang == nil or chunk.Language == lang then
		chunk:AddValue("APLog", "APLog will show here." .. string.rep(" ", 475))
		
		chunk:AddValue("APWrench", "XX")
		chunk:AddValue("APHnR", "XX")
		
		chunk:AddValue("APMaxCoins", "       ")
		
		chunk:AddValue("APProgressTitle", "Goals:")
		chunk:AddValue("APProgress", "AP NOT LOADED" .. string.rep(" ", 475))
		
		local translations = TranslationMap[chunk.Language]
		if translations then
			for k,v in pairs(translations) do
				chunk:SetValue(k, v)
			end
		end
	end
end

TextBibleCache = tostring(P3DFile)
Output(TextBibleCache)