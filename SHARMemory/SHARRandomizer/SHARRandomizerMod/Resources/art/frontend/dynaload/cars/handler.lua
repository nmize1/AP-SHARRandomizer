local function RemoveChunksWithName(Parent, RemoveNames, Identifier)
	for idx, chunk in Parent:GetChunksIndexed(Identifier, true) do
		if RemoveNames[chunk.Name] then
			Parent:RemoveChunk(idx)
		end
	end
end

--
-- Handle Radical's Hardcoded Shit
--

local RemoveAllWheels = 
{
	["frink_v"] = true,
	["honor_v"] = true,
	["hbike_v"] = true,
	["witchcar"] = true,
	["ship"] = true,
	["mono_v"] = true,
}

local RemoveFrontWheels = 
{
	["rocke_v"] = true,
}

--
-- Get Paths
--

local Path = GetPath()

local FileName = GetFileName(Path)

local FileNameWithoutExtension = RemoveFileExtension(FileName)

local BaseFilePath = "/GameData/art/cars/" .. FileName

--
-- Load P3D File
--

if not Exists(BaseFilePath, true, false) then
	print("Could not file car model:" .. BaseFilePath)
	return
end

local P3DFile = P3D.P3DFile(BaseFilePath)

if P3DFile == nil then
	print("Could not load car model:" .. BaseFilePath)
	return
end

--
-- Get Chunks
--

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

--
-- Remove Dummy Wheel Models (Radical hardcodedly removes these in-game)
--

if RemoveAllWheels[FileNameWithoutExtension] then
	RemoveChunksWithName(CompositeDrawablePropListChunk, { ["wShape0"] = true, ["wShape1"] = true, ["wShape2"] = true, ["wShape3"] = true }, P3D.Identifiers.Composite_Drawable_Prop)
elseif RemoveFrontWheels[FileNameWithoutExtension] then
	RemoveChunksWithName(CompositeDrawablePropListChunk, { ["wShape2"] = true, ["wShape3"] = true }, P3D.Identifiers.Composite_Drawable_Prop)
end

--
-- Remove Billboards
--

local removedBillboards = {}

for idx, chunk in P3DFile:GetChunksIndexed(P3D.Identifiers.Old_Billboard_Quad_Group, true) do
	removedBillboards[chunk.Name] = true

	P3DFile:RemoveChunk(idx)
end

RemoveChunksWithName(CompositeDrawablePropListChunk, removedBillboards)

--
-- Remove Old Frame Controllers
--

for idx in P3DFile:GetChunksIndexed(P3D.Identifiers.Old_Frame_Controller, true) do
	P3DFile:RemoveChunk(idx)
end

--
-- Remove Multi Controller Tracks
--

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

--
-- Correct Shader Names
--

for chunk in P3DFile:GetChunks(P3D.Identifiers.Shader) do
	if chunk.PddiShaderName == "environment" or chunk.PddiShaderName == "spheremap" then
		chunk.PddiShaderName = "simple"
	end
end

--
-- Output P3D File
--

P3DFile:Output()