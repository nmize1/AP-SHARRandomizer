local Path = GetPath()
local GamePath = GetGamePath(Path)

local P3DFile = P3D.P3DFile(GamePath)

local Skeleton = P3DFile:GetChunk(P3D.Identifiers.Skeleton)
if not Skeleton then
	return
end

Skeleton:AddChunk(P3D.SkeletonJointP3DChunk("dl", 0, -1, -1, -1, -1, -1, P3D.Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -0.18, -0.0539, 0.0639, 1)))
Skeleton:AddChunk(P3D.SkeletonJointP3DChunk("pl", 0, -1, -1, -1, -1, -1, P3D.Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0.55, -0.0539, 0.0639, 1)))

P3DFile:Output()