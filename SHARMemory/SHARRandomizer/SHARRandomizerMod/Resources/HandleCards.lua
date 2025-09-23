local P3DFile = P3D.P3DFile()

local Cards = Config.CARD

local level = GetPath():match("level0(%d)")
level = tonumber(level)

local startIndex = (level - 1) * 7
for cardIndex=1,7 do
	local card = Cards[startIndex + cardIndex]

	local locator = P3D.LocatorP3DChunk(card.Name, P3D.Vector3(card.X, card.Y, card.Z), 9, card.Name, card.Name, "CollectorCard", 3, 0)
	P3DFile:AddChunk(locator)

	local trigger = P3D.TriggerVolumeP3DChunk(
		card.Name .. "Trigger",
		0,
		{ X = 2.5, Y = 2.5, Z = 2.5 },
		{
			M11 = 1, M12 = 0, M13 = 0, M14 = 0,
			M21 = 0, M22 = 1, M23 = 0, M24 = 0,
			M31 = 0, M32 = 0, M33 = 1, M34 = 0,
			M41 = card.X, M42 = card.Y, M43 = card.Z, M44 = 1
		}
	)
	locator:AddChunk(trigger)
end

P3DFile:Output()