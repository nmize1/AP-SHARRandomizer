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

local ids = Config.IDENTIFIER

local TranslationMap = {
	["E"] = {
		["SKINN_V"] = "Skinner's Sedan",
		["TT"] = "Audi TT",
		["MOE_V"] = "Moe's Sedan",
		["CBONE"] = "Bonestorm Truck",
		["GLASTRUC"] = "Glass Truck",
		["SAVE_GAME"] = "AUTO-SAVED ON SERVER",
		["NEW_GAME"] = "Client Not Connected\n          ",
	},
	["F"] = {
		["SKINN_V"] = "La Berline de Skinner",
		["TT"] = "Audi TT",
		["MOE_V"] = "La Berline de Moe",
		["CBONE"] = "Camion Bonestorm",
		["GLASTRUC"] = "Camion de vitrier",
		["SAVE_GAME"] = "AUTO-ENREGISTRÉ SUR SERVEUR",
		["NEW_GAME"] = "Client Déconnecté\n               ",
	},
	["G"] = {
		["SKINN_V"] = "Skinners Wagen",
		["TT"] = "Audi TT",
		["MOE_V"] = "Moes Wagen",
		["CBONE"] = "Bonestorm-Laster",
		["GLASTRUC"] = "Glaswagen",
		["SAVE_GAME"] = "AUTO-GESPEICHERT AUF SERVER",
		["NEW_GAME"] = "Client nicht Verbunden\n          ",
	},
	["S"] = {
		["SKINN_V"] = "El Sedán de Skinner",
		["TT"] = "Audi TT",
		["MOE_V"] = "El Sedán de Moe",
		["CBONE"] = "Camión Bonestorm",
		["GLASTRUC"] = "Camión de cristales",
		["SAVE_GAME"] = "AUTOGUARDADO EN SERVIDOR",
		["NEW_GAME"] = "Cliente no Connectado\n          ",
	},
}

local UnlockTranslation = {
	["E"] = "You need to unlock \"%s\".",
	["F"] = "Vous devez débloquer \"%s\".",
	["G"] = "Sie müssen \"%s\" entsperren.",
	["S"] = "Debes desbloquear \"%s\".",
}
local default = "If you can read this, then you are not running the Archipelago client. You can probably also access a bunch of other things you should not do."

for chunk in BibleChunk:GetChunks(P3D.Identifiers.Frontend_Language) do
	if lang == nil or chunk.Language == lang then
		for i=1,42 do
			chunk:AddValue("APCAR" .. i, default)
		end

		chunk:AddValue("APLog", ids[1].TitleID .. string.rep(" ", 496 - #ids[1].TitleID))
		chunk:AddValue("VerifyID", ids[1].ID)
		
		chunk:AddValue("APWrench", "XXX")
		chunk:AddValue("APHnR", "XXX")
		
		chunk:AddValue("APMaxCoins", "XXXXXXXXXXXXXXX")
		
		chunk:AddValue("APProgressTitle", "Goals:")
		chunk:AddValue("APProgress", "AP NOT LOADED" .. string.rep(" ", 475))
		
		local translations = TranslationMap[chunk.Language]
		if translations then
			for k,v in pairs(translations) do
				chunk:SetValue(k, v)
			end
		end

		for k, v in pairs(MissionLock) do				
			local success, displayName = pcall(chunk.GetValueFromName, chunk, k:upper())
			if not success then
				displayName = k
			end
			
			local message = string.format(UnlockTranslation[chunk.Language], displayName)
			if k == "fakecar" then
				message = "Welcome to The Simpsons Hit & Run AP.\nYour progress is saved automatically on the AP server." ..
						  "You can switch to any level\nfrom mission select, even when locked.\n\nThanks for playing, enjoy!"
			end
			chunk:AddValue("INGAME_MESSAGE_" .. v.IngameMessageIdx, message)
			if k == "fakecar" then
				message = "Welcome to The Simpsons Hit & Run AP! Enjoy!"
			end
			chunk:AddValue("MISSION_OBJECTIVE_" .. v.MissionObjectiveIdx, message)
		end

		--allocate longer level and mission names
		for i=1,7 do
			local entry = "LEVEL_" .. i
			local value = chunk:GetValueFromName(entry)
			chunk:SetValue(entry, value .. string.rep(" ", math.max(0, 50 - #value)))
			for j=1,7 do
				local entry = "MISSION_TITLE_L" .. i .. "_M" .. j
				local value = chunk:GetValueFromName(entry)
				chunk:SetValue(entry, value .. string.rep(" ", math.max(0, 50 - #value)))
			end

			local race = "RACE_COMPLETE_INFO_ALL_" .. i
			local value = chunk:GetValueFromName(race)
			chunk:SetValue(race, value .. string.rep(" ", math.max(0, 500 - #value)))
		end
	end
end



TextBibleCache = tostring(P3DFile)
Output(TextBibleCache)