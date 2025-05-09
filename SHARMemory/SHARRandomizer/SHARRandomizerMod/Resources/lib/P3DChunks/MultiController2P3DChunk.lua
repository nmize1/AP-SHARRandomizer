--[[
CREDITS:
	Proddy#7272				- Converting to Lua
	luca$ Cardellini#5473	- P3D Chunk Structure
]]

local P3D = P3D
assert(P3D and P3D.ChunkClasses, "This file must be called after P3D2.lua")
assert(P3D.MultiController2P3DChunk == nil, "Chunk type already loaded.")

local string_format = string.format
local string_pack = string.pack
local string_rep = string.rep
local string_reverse = string.reverse
local string_unpack = string.unpack

local table_concat = table.concat
local table_unpack = table.unpack

local assert = assert
local tostring = tostring
local type = type

local function new(self, Version, Name, CycleMode, NumCycles, InfiniteCycle, NumFrames, FrameRate)
	assert(type(Version) == "number", "Arg #1 (Version) must be a number.")
	assert(type(Name) == "string", "Arg #2 (Name) must be a string.")
	assert(type(CycleMode) == "string", "Arg #3 (CycleMode) must be a string.")
	assert(type(NumCycles) == "number", "Arg #4 (NumCycles) must be a number.")
	assert(type(InfiniteCycle) == "number", "Arg #5 (InfiniteCycle) must be a number.")
	assert(type(NumFrames) == "number", "Arg #6 (NumFrames) must be a number.")
	assert(type(FrameRate) == "number", "Arg #7 (FrameRate) must be a number.")

	local Data = {
		Endian = "<",
		Chunks = {},
		Version = Version,
		Name = Name,
		CycleMode = CycleMode,
		NumCycles = NumCycles,
		InfiniteCycle = InfiniteCycle,
		NumFrames = NumFrames,
		FrameRate = FrameRate,
	}
	
	self.__index = self
	return setmetatable(Data, self)
end

P3D.MultiController2P3DChunk = P3D.P3DChunk:newChildClass(P3D.Identifiers.Multi_Controller_2)
P3D.MultiController2P3DChunk.new = new
function P3D.MultiController2P3DChunk:parse(Endian, Contents, Pos, DataLength)
	local chunk = self.parentClass.parse(self, Endian, Contents, Pos, DataLength, self.Identifier)
	
	chunk.Version, chunk.Name, chunk.CycleMode, chunk.NumCycles, chunk.InfiniteCycle, chunk.NumFrames, chunk.FrameRate = string_unpack(Endian .. "Is1c4IIff", chunk.ValueStr)
	chunk.Name = P3D.CleanP3DString(chunk.Name)
	if Endian == ">" then
		chunk.CycleMode = string_reverse(chunk.CycleMode)
	end

	return chunk
end

function P3D.MultiController2P3DChunk:GetNumTracks()
	local n = 0
	for i=1,#self.Chunks do
		if self.Chunks[i].Identifier == P3D.Identifiers.Multi_Controller_Track then
			n = n + 1
		end
	end
	return n
end

function P3D.MultiController2P3DChunk:__tostring()
	local chunks = {}
	for i=1,#self.Chunks do
		chunks[i] = tostring(self.Chunks[i])
	end
	local chunkData = table_concat(chunks)
	
	local Name = P3D.MakeP3DString(self.Name)
	local CycleMode = self.CycleMode
	if self.Endian == ">" then
		CycleMode = string_reverse(CycleMode)
	end
	
	local headerLen = 12 + 4 + #Name + 1 + 4 + 4 + 4 + 4 + 4 + 4
	return string_pack(self.Endian .. "IIIIs1c4IIffI", self.Identifier, headerLen, headerLen + #chunkData, self.Version, Name, CycleMode, self.NumCycles, self.InfiniteCycle, self.NumFrames, self.FrameRate, self:GetNumTracks()) .. chunkData
end