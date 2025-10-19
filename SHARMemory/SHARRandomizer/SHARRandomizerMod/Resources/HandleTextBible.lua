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
		["CBONE"] = "Bonestorm Truck",
		["GLASTRUC"] = "Glass Truck",
		["SAVE_GAME"] = "AUTO-SAVED ON SERVER",
	},
	["F"] = {
		["SKINN_V"] = "La Berline de Skinner",
		["MOE_V"] = "La Berline de Moe",
		["CBONE"] = "Camion Bonestorm",
		["GLASTRUC"] = "Camion de vitrier",
		["SAVE_GAME"] = "AUTO-ENREGISTRÉ SUR SERVEUR",
	},
	["G"] = {
		["SKINN_V"] = "Skinners Wagen",
		["MOE_V"] = "Moes Wagen",
		["CBONE"] = "Bonestorm-Laster",
		["GLASTRUC"] = "Glaswagen",
		["SAVE_GAME"] = "AUTO-GESPEICHERT AUF SERVER",
	},
	["S"] = {
		["SKINN_V"] = "El Sedán de Skinner",
		["MOE_V"] = "El Sedán de Moe",
		["CBONE"] = "Camión Bonestorm",
		["GLASTRUC"] = "Camión de cristales",
		["SAVE_GAME"] = "AUTOGUARDADO EN SERVIDOR",
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

		for k, v in pairs(MissionLock) do
			chunk:AddValue("MISSION_OBJECTIVE_" .. v.MissionObjectiveIdx, "You need to unlock " .. k)
			chunk:AddValue("INGAME_MESSAGE_" .. v.IngameMessageIdx, "You need to unlock " .. k)
		end
	end
end

TextBibleCache = tostring(P3DFile)
Output(TextBibleCache)