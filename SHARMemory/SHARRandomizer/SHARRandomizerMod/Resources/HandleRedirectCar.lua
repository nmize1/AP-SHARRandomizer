local CarName = RemoveFileExtension(GetFileName(GetPath()))
local ExcludedCars = {
    ["huskA"] = true,
    ["common"] = true,
}
if ExcludedCars[CarName] then
    return
end
print("Redirecting \"" .. CarName .. "\" to redbrick.")

local P3DFile = P3D.P3DFile("/GameData/art/cars/redbrick.p3d")

local RenameChunks = {
    [P3D.Identifiers.Multi_Controller] = true,
    [P3D.Identifiers.Physics_Object] = true,
    [P3D.Identifiers.Collision_Object] = true,
    [P3D.Identifiers.Skeleton] = true,
}

for chunk in P3DFile:GetChunks() do
    if RenameChunks[chunk.Identifier] then
        if string.match(chunk.Name, "BV$") then
            chunk.Name = CarName .. "BV"
        else
            chunk.Name = CarName
        end
    end
end

local CompositeDrawable = P3DFile:GetChunk(P3D.Identifiers.Composite_Drawable)
CompositeDrawable.Name = CarName
CompositeDrawable.SkeletonName = CarName

local OldFrameController = P3DFile:GetChunk(P3D.Identifiers.Old_Frame_Controller)
if OldFrameController then
    OldFrameController.HierarchyName = CarName
end

P3DFile:Output()