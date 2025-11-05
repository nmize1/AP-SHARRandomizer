local Path = GetPath()
local GamePath = GetGamePath(Path)

local SPT = SPTParser.SPTFile(GamePath)

for carSoundParameters in SPT:GetClasses("carSoundParameters") do
	for method, index in carSoundParameters:GetMethods(true) do
		local name = method.Name
		if name == "SetEngineClipName" or name == "SetEngineIdleClipName" then
			if method.Parameters[1] == "tt" then
				method.Parameters[1] = "apu_car"
			end
		elseif name == "SetOverlayClipName" and method.Parameters[1] == "generator" then -- Fucking Radical and their broken ass Monorail
			carSoundParameters:RemoveMethod(index)
		end
	end
end

local redbrick = SPT:GetClass("carSoundParameters", false, "redbrick")
for i=1,21 do
	local class = SPTParser.Class("carSoundParameters", "APCar" .. i)
	
	for method in redbrick:GetMethods() do
		class:AddMethod(method.Name, method.Parameters, method.Option)
	end
	
	SPT.Classes[#SPT.Classes + 1] = class
end

local BadOverlayClips = {
    [""] = true, -- What the fuck Radical
    --["generator"] = true, -- Broken ass Monorail
}
for carSoundParameters in SPT:GetClasses("carSoundParameters") do
    for method, index in carSoundParameters:GetMethods(true) do
        local name = method.Name
        if name == "SetEngineClipName" or name == "SetEngineIdleClipName" then
            if method.Parameters[1] == "tt" then
                method.Parameters[1] = "apu_car"
            end
        elseif name == "SetOverlayClipName" then
            if BadOverlayClips[method.Parameters[1]] then
                carSoundParameters:RemoveMethod(index)
            end
        end
    end
end

Output(tostring(SPT))