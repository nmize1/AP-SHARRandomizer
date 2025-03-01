local P3DFile = P3D.P3DFile(GetModPath() .. "/Resources/redbrick.p3d")
local Skeleton = P3DFile:GetChunk(P3D.Identifiers.Skeleton)
Skeleton.Name = RemoveFileExtension(GetFileName(GetPath()))
local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
CompositeDrawable.Name = RemoveFileExtension(GetFileName(GetPath()))
local MultiController = P3DFile:GetChunk(P3D.Identifiers.MultiController)
MultiController.Name = RemoveFileExtension(GetFileName(GetPath()))
local Collision = P3DFile:GetChunk(P3D.Identifiers.Collision)
Collision.Name = RemoveFileExtension(GetFileName(GetPath()))


P3DFile:Output()