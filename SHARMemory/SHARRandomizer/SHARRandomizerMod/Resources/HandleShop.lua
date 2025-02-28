local P3DFile = P3D.P3DFile(GetModPath() .. "/Resources/ShopModel.p3d")
local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
CompositeDrawable.Name = RemoveFileExtension(GetFileName(GetPath()))
P3DFile:Output()