local P3DFile = P3D.P3DFile("/GameData/art/cars/redbrick.p3d")
local CarName = RemoveFileExtension(GetFileName(GetPath()))
local Skeleton = P3DFile:GetChunk(P3D.Identifiers.Skeleton)
Skeleton.Name = CarName
local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
CompositeDrawable.Name = CarName
CompositeDrawable.SkeletonName = CarName
local MultiController = P3DFile:GetChunk(P3D.Identifiers.MultiController)
MultiController.Name = CarName
local Collision = P3DFile:GetChunk(P3D.Identifiers.Collision)
Collision.Name = CarName
local OldFrameController = P3DFile:GetChunk(P3D.Identifiers.OldFrameController)
OldFrameController.Name = CarName

P3DFile:Output()