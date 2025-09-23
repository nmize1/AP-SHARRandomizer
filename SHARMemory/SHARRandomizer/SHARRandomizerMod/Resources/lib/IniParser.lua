local assert = assert
local type = type

local Exists = Exists
local ReadFile = ReadFile

local string_gmatch = string.gmatch
local string_gsub = string.gsub
local string_match = string.match
local string_sub = string.sub
local string_unpack = string.unpack
local table_concat = table.concat
local utf8_char = utf8.char

local fmtMap = {
    ["\xFF\xFE"] = "<H", -- UTF-16LE
    ["\xFE\xFF"] = ">H", -- UTF-16BE
}

local function ReadTextFile(path)
    local content = ReadFile(path)
    if not content then
        return nil
    end
    
    local contentN = #content
    if contentN == 0 then
        return ""
    end
    
    if contentN >= 3 and content:sub(1, 3) == "\xEF\xBB\xBF" then -- UTF-8 BOM
        return content:sub(4)
    end
    
    if contentN >= 2 then -- UTF-16
        local fmt = fmtMap[content:sub(1, 2)]
        
        if fmt then
            local out = {}
            local outN = 0
            local i = 3
            
            local codepoint
            while i <= contentN do
                codepoint, i = string_unpack(fmt, content, i)
                
                -- Handle surrogate pairs
                if codepoint >= 0xD800 and codepoint <= 0xDBFF and i + 1 <= contentN then
                    local low, ni2 = string_unpack(fmt, content, i)
                    if low >= 0xDC00 and low <= 0xDFFF then
                        codepoint = 0x10000 + ((codepoint - 0xD800) * 0x400) + (low - 0xDC00)
                        i = ni2
                    end
                end
                
                outN = outN + 1
                out[outN] = utf8_char(codepoint)
            end

            return table_concat(out)
        end
    end
    
    return content -- Assume normal UTF-8
end

local function UnescapeString(s)
    s = string_gsub(s, "\\\\", "{BACKSLASH}")
    s = string_gsub(s, "\\n", "\n")
    s = string_gsub(s, "\\t", "\t")
    s = string_gsub(s, "\\r", "\r")
    s = string_gsub(s, '\\"', '"')
    s = string_gsub(s, "\\'", "'")
    s = string_gsub(s, "{BACKSLASH}", "\\")
    return s
end

local CommentChars = {
    ["#"] = true,
    [";"] = true,
}

function IniParser(Path)
    local Contents = ReadTextFile(Path)
    Contents = string_gsub(Contents, "\r\n", "\n")
    
    local Out = {}
    
    local CurrentHeader
    for line in string_gmatch(Contents, "[^\n]+") do
        if not CommentChars[string_sub(line, 1, 1)] then
            local HeaderName = string_match(line, "^%[([^%]]+)%]$")
            if HeaderName then
                CurrentHeader = {}
                
                local Headers = Out[HeaderName]
                if Headers == nil then
                    Headers = {}
                    Out[HeaderName] = Headers
                end
                Headers[#Headers + 1] = CurrentHeader
            elseif CurrentHeader then
                local Key, Value = string_match(line, "^(.-)%s*=%s*(.+)$")
                if Key and Value then
                    Key = UnescapeString(Key)
                    local ValueStr = string_match(Value, '^"([^"]+)"$') or string_match(Value, "^'([^']+)'$")
                    if ValueStr then
                        CurrentHeader[Key] = UnescapeString(ValueStr)
                    else
                        Value = Value:match("^(.-)%s*[;#].*$") or Value
                        if Value == "true" then
                            CurrentHeader[Key] = true
                        elseif Value == "false" then
                            CurrentHeader[Key] = false
                        else
                            CurrentHeader[Key] = tonumber(Value) or Value
                        end
                    end
                end
            end
        end
    end
    
    return Out
end