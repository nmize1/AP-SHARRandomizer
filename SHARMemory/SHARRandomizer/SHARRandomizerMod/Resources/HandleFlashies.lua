local Path = GetPath()

local FileName = GetFileName(Path)

local FileNameWithoutExtension = RemoveFileExtension(FileName)

local BaseFilePath = "/GameData/art/cars/" .. FileName

local function RemoveChunksWithName(Parent, RemoveNames, Identifier)
	for idx, chunk in Parent:GetChunksIndexed(Identifier, true) do
		if RemoveNames[chunk.Name] then
			Parent:RemoveChunk(idx)
		end
	end
end


if not Exists(BaseFilePath, true, false) then
	print("Could not file car model:" .. BaseFilePath)
	return
end

local P3DFile = P3D.P3DFile(BaseFilePath)

if P3DFile == nil then
	print("Could not load car model:" .. BaseFilePath)
	return
end

local CompositeDrawableChunk = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)

if CompositeDrawableChunk == nil then
	print("Could not find Composite Drawable for car: " .. FileNameWithoutExtension)
	return
end

local CompositeDrawablePropListChunk = CompositeDrawableChunk:GetChunk(P3D.Identifiers.Composite_Drawable_Prop_List)

if CompositeDrawablePropListChunk == nil then
	print("Could not find Composite Drawable Prop List for car: " .. FileNameWithoutExtension)
	return
end

local removedBillboards = {}

for idx, chunk in P3DFile:GetChunksIndexed(P3D.Identifiers.Old_Billboard_Quad_Group, true) do
	removedBillboards[chunk.Name] = true

	P3DFile:RemoveChunk(idx)
end

RemoveChunksWithName(CompositeDrawablePropListChunk, removedBillboards)


local MultiControllerChunk = P3DFile:GetChunk(P3D.Identifiers.Multi_Controller)

if MultiControllerChunk ~= nil then
	local MultiControllerTracksChunk = MultiControllerChunk:GetChunk(P3D.Identifiers.Multi_Controller_Tracks)

	if MultiControllerTracksChunk then
		local Tracks = MultiControllerTracksChunk.Tracks

		for i = #Tracks, 1, -1 do
			if Tracks[i].Name:sub(1, 4) ~= "PTRN" then
				table.remove(Tracks, i)
			end
		end
	end
end

if Settings.RemoveFlashingLights then
	print("Removing flashing lights from " .. Path)
	P3DFile:Output()
end