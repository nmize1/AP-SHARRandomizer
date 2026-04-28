local Path = GetPath()
local GamePath = "/GameData/" .. Path
local P3DFile = P3D.P3DFile(GamePath)

local Level = GetCurrentLevel()

--local LocatorChunk = P3D.LocatorP3DChunk("Lobby", P3D.Vector3(0, 0, 0), 5, "archroom.p3d;doorbells.p3d;doorplates.p3d;")
--LocatorChunk:AddChunk(P3D.TriggerVolumeP3DChunk("Lobby", 0, P3D.Vector3(1, 1, 1), P3D.Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)))

local SVTLocatorChunk = P3D.LocatorP3DChunk("counter", P3D.Vector3(992.92, 0,  997.10), 2)


--P3DFile:AddChunk(LocatorChunk)
P3DFile:AddChunk(SVTLocatorChunk)
P3DFile:Output()