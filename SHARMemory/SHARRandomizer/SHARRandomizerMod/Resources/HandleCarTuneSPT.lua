local spt = ReadFile("/GameData/" .. GetPath())
Output(spt)

local redbrick = spt:match("create carSoundParameters named redbrick\r\n{(.-)}")
for i=1,21 do
    Output("create carSoundParameters named APCar" .. i .. "\r\n{")
    Output(redbrick)
    Output("}\r\n")
end