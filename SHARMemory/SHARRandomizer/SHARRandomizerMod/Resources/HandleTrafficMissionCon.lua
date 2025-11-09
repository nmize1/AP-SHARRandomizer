local Path = GetPath()

local car = Path:match("([^\\/]+)%.con")

local CONFile = ReadFile("//GameData//scripts//cars//" .. car .. ".con")
local CON = MFKLexer.Lexer:Parse(CONFile)

for i=1,#CON.Functions do
	local func = CON.Functions[i]
	local name = func.Name:lower()

	if name == "SetHitPoints" then
		local hp = func.Arguments[1]
		if hp > 12 then
			func.Arguments[1] = 12
		end
	end
end

CON:Output(true)