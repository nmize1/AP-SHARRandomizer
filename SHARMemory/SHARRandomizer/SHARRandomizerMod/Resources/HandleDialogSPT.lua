local Path = GetPath()
local GamePath = "/GameData/" .. Path

print("Handling dialog SPT: " .. Path)

local contents = ReadFile(GamePath)

contents = contents:gsub("Zm2_L7R1", "Hom_L7")

Output(contents)

Output([[

create daSoundResourceData named W_Doorbell_Lvl1_Archipelago
{
    AddFilename ( "Level1/W_Doorbell_Lvl1_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl2_Archipelago
{
    AddFilename ( "Level2/W_Doorbell_Lvl2_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl3_Archipelago
{
    AddFilename ( "Level3/W_Doorbell_Lvl3_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl4_Archipelago
{
    AddFilename ( "Level4/W_Doorbell_Lvl4_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl5_Archipelago
{
    AddFilename ( "Level5/W_Doorbell_Lvl5_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl6_Archipelago
{
    AddFilename ( "Level6/W_Doorbell_Lvl6_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}
create daSoundResourceData named W_Doorbell_Lvl7_Archipelago
{
    AddFilename ( "Level7/W_Doorbell_Lvl7_Archipelago.rsd" 1.000000 )
    SetStreaming ( true )
}]])