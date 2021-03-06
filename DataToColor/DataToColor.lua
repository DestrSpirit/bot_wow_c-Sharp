local L = TomTomLocals
local ldb = LibStub("LibDataBroker-1.1")
local hbd = LibStub("HereBeDragons-2.0")

local addonName, addon = ...
local TomTom = addon
local who_is_attacking_me = {}
local current_time
--local CHARACTER_NA--ME = UnitName("player")

addon.hbd = hbd
addon.CLASSIC = math.floor(select(4, GetBuildInfo() ) / 100) == 113

local loop = CreateFrame("Frame");

--КООРДИНАТЫ 1 XX.**
local Pixelxyr1 = CreateFrame("Frame", "pixelxyr1", UIParent) 
Pixelxyr1:SetSize(1, 1) 
Pixelxyr1:SetPoint("TOPLEFT", 0, 0);

local Pixelxyr1Texture = Pixelxyr1:CreateTexture() 
Pixelxyr1Texture:SetAllPoints() 
Pixelxyr1Texture:SetColorTexture(255, 0, 0, 1)

--КООРДИНАТЫ 2 **.XX
local Pixelxyr2 = CreateFrame("Frame", "pixelxyr2", UIParent) 
Pixelxyr2:SetSize(1, 1) 
Pixelxyr2:SetPoint("TOPLEFT", 1, 0);

local Pixelxyr2Texture = Pixelxyr2:CreateTexture() 
Pixelxyr2Texture:SetAllPoints() 
Pixelxyr2Texture:SetColorTexture(0, 255, 0, 1)

--ТАРГЕТ ИД (12,34,56)
local PixelID = CreateFrame("Frame", "pixelid", UIParent)
PixelID:SetSize(1, 1)
PixelID:SetPoint("TOPLEFT", 3, 0)

local PixelIDTexture = PixelID:CreateTexture()
PixelIDTexture:SetAllPoints()
PixelIDTexture:SetColorTexture(255, 0, 255, 1)

--БАФЫ, КД, ХП-ШКИ
local PixelData = CreateFrame("Frame", "pixeldata", UIParent)
PixelData:SetSize(1, 1)
PixelData:SetPoint("TOPLEFT", 5, 0)

local PixelDataTexture = PixelData:CreateTexture()
PixelDataTexture:SetAllPoints()
PixelDataTexture:SetColorTexture(255, 0, 255, 1)

--ТРУПНЫЕ ДЕЛА x.* + BOOLs
local PixelDead1 = CreateFrame("Frame", "PixelDead1", UIParent) 
PixelDead1:SetSize(1, 1) 
PixelDead1:SetPoint("TOPLEFT", 0, -1);

local PixelDead1Texture = PixelDead1:CreateTexture() 
PixelDead1Texture:SetAllPoints() 
PixelDead1Texture:SetColorTexture(0, 0, 255, 1)

--ТРУПНЫЕ ДЕЛА *.x
local PixelDead2 = CreateFrame("Frame", "PixelDead2", UIParent)
PixelDead2:SetSize(1, 1)
PixelDead2:SetPoint("TOPLEFT", 0, -2)

local PixelDead2Texture = PixelDead2:CreateTexture()
PixelDead2Texture:SetAllPoints()
PixelDead2Texture:SetColorTexture(255, 0, 255, 1)


function Log( text )
	SELECTED_CHAT_FRAME:AddMessage( text );
end

function loop:onUpdate()		
		-- КООРДИНАТЫ!
		local x, y, r = GetCurrentCoords()
		-- XX.**, YY,**, R,**
		c_x1 = ctp(floor(x*100))
		c_y1 = ctp(floor(y*100))
		c_r1 = ctp(floor(r*10))
		Pixelxyr1Texture:SetColorTexture(c_x1,c_y1,c_r1,1)
		
		-- **.XX, **.YY, **.R
		c_x1 = ctp(cof(math.fmod(x*100,1)*100))
		c_y1 = ctp(cof(math.fmod(y*100,1)*100))	
		c_r1 = ctp(math.fmod(r*10,1)*10)
		
		Pixelxyr2Texture:SetColorTexture(c_x1,c_y1,c_r1,1)
		
		--ТАРГЕТ ID (12, 34, 56)
		t_id = tostring(getTargetUUID())
		if t_id == "0" then t_id = "000000" end
		t_id = string.sub(t_id, -6)
		
		-- (12, **, **)
		t_id_r = ctp(tonumber(string.sub(t_id,-6, -5)))
		
		-- (**, 34, **)
		t_id_g = ctp(tonumber(string.sub(t_id,-4, -3)))
		
		-- (**, **, 56)
		t_id_b = ctp(tonumber(string.sub(t_id, -2)))
		
		PixelIDTexture:SetColorTexture(t_id_r, t_id_g, t_id_b, 1)
		--s_b = (LeBytesToInt(BytesSelfInfo()))
		
		--БАФФЫ
		zxc_r = ctp(LeBytesToInt(BytesSelfInfo()))
		
		-- КОЛЛДАУНЫ
		zxc_g = ctp(LeBytesToInt(BytesActionsInfo()))
		
		-- ХПШКИ
		zxc_b = ctp(selfHpPercent())
		PixelDataTexture:SetColorTexture(zxc_r, zxc_g, zxc_b, 1)

		--ВОЗРОЖДАЕМСЯ
		RetrieveCorpse()
		RepopMe()
		
		-- XX.**, YY,** , bools
		c_x1 = ctp(floor(CorpsePosition("x")*100))
		c_y1 = ctp(floor(CorpsePosition("y")*100))
		c_r1 = ctp(LeBytesToInt(BytesBoolInfo()))
		PixelDead1Texture:SetColorTexture(c_x1, c_y1, c_r1, 1)
		
		--**.XX, **.YY
		c_x1 = ctp(cof(math.fmod(CorpsePosition("x")*100,1)*100))
		c_y1 = ctp(cof(math.fmod(CorpsePosition("y")*100,1)*100))	
		
		PixelDead2Texture:SetColorTexture(c_x1, c_y1, ctp(GetComboPoints("player", "target")), 1)	
end


function BytesBoolInfo()
	local si = 0
	si = si*10+TargetCanAttach() 							--0
    si = si*10+TargetIsDead()								--1
    si = si*10+targetCombatStatus()							--2
    si = si*10+isInRange()									--3
    si = si*10+playerCombatStatus()							--4
    si = si*10+IsTargetOfTargetPlayer()						--5
	si = si*10+isBroken()									--6
	si = si*10+deadOrAlive()								--7
	return si
end

-- --Druid cat
-- function BytesSelfInfo()
	-- local si = 0
	-- si = si*10+GetBuffs("Облик кошки")						--0
    -- si = si*10+GetBuffs("Крадущийся зверь")					--1
    -- si = si*10+GetBuffs("Ясность мысли")					--2
    -- si = si*10+GetBuffs("Кровавые когти")					--3
    -- si = si*10+GetBuffs("Тигриное неистовство")				--4
    -- si = si*10+GetBuffs("Стремительность хищника")			--5
	-- si = si*10+GetTBuffs("Глубокая рана")					--6
	-- si = si*10+GetBuffs("Походный облик")					--7
	-- return si
-- end

-- function BytesActionsInfo()
	-- local si = 0
	-- si = si*10+ActionMeleeCheck("Глубокая рана") 			--0
    -- si = si*10+ActionMeleeCheck("Полоснуть")				--1	
    -- si = si*10+ActionCheck("Жестокий удар когтями")			--2
    -- si = si*10+ActionMeleeCheck("Стремительный рывок")		--3
    -- si = si*10+ActionCheck("Тигриное неистовство")			--4
    -- si = si*10+ActionMeleeCheck("Свирепый укус")			--5
	-- si = si*10+ActionCheck("Дубовая кожа")					--6
	-- si = si*10+ActionCheck("Крадущийся зверь")				--7
	-- return si
-- end

function BytesSelfInfo()
	local si = 0
	si = si*10+GetBuffs("Незаметность")						--0
    si = si*10+GetBuffs("Быстродействующий яд")					--1
    si = si*10+GetBuffs("Символы смерти")					--2
    si = si*10+GetBuffs("Танец теней")					--3
    si = si*10+GetBuffs("Мясорубка")				--4
    si = si*10+GetBuffs("Стремительность хищника")			--5
	si = si*10+GetTBuffs("Глубокая рана")					--6
	si = si*10+GetTBuffs("Взбучка")							--7
	return si
end

function BytesActionsInfo()
	local si = 0
	si = si*10+ActionCheck("Удар в спину") 			--0
    si = si*10+ActionMeleeCheck("Потрошение")				--1	
    si = si*10+ActionCheck("Удар тьмы")						--2
    si = si*10+ActionCheck("Символы смерти")				--3
    si = si*10+ActionCheck("Танец теней")					--4
    si = si*10+ActionCheck("Шаг сквозь тень")				--5
	si = si*10+ActionCheck("Алый фиал")					    --6
	si = si*10+ActionCheck("Незаметность")					--7
	return si	
end

function selfHpPercent()
	v = UnitHealth("player")/UnitHealthMax("player")*100
	return v
end

function ActionMeleeCheck(i)
	local xxx = GetSpellCooldown(i)
	if xxx==0 and IsUsableSpell(i) and IsSpellInRange(i, "target") == 1 then
		return 1
	end
	return 0
end

function ActionCheck(i)
	local xxx = GetSpellCooldown(i)
	if xxx==0 and IsUsableSpell(i) then
		return 1
	end
	return 0
end

function LeBytesToInt(v)
	local b = 0
	for i=1, 8 do
		local x = 8 - i
		local y = math.pow(10, x)
		local z = math.floor(v/y)
		b = b + z*math.pow(2,x)
		if z == 1 then v = v - y end
		if v == 0 then break end	
	end
	return b
end

function GetBuffs(buff)
    for i = 1, 20 do
        local b = UnitBuff("player", i)
        if b ~= nil then
            if string.find(b, buff) then
                return 1
            end
        end
    end
    return 0
end 

function GetTBuffs(buff)
    for i = 1, 20 do
        local b = UnitDebuff("target", i)
        if b ~= nil then
            if string.find(b, buff) then
                return 1
            end
        end
    end
    return 0
end

function cof(x)
	if math.fmod(x,1) > 0.5 then return math.ceil(x) end
	return math.floor(x)
end

function ctp(c)
	return c/255
end


function TargetIsDead()
    local targStatus = UnitIsDead("target")
    if targStatus then
        return 1
    else
        return 0
    end
end


function TargetIsEnemy()
    local t = UnitIsEnemy("player", "target")
    if t then
        return 1
    else
        return 0
    end
end


function TargetIsPlayer()
    local t = UnitIsPlayer("target")
    if t then
        return 1
    else
        return 0
    end
end

function GetCurrentCoords()
	local x, y = hbd:GetPlayerZonePosition()
	local z = GetPlayerFacing()
	if x and y and x > 0 and y > 0 then
		return x, y, z
	end
end

-- Returns true if target of our target is us
function IsTargetOfTargetPlayer()
    if CHARACTER_NAME == UnitName("targettarget") and CHARACTER_NAME ~= UnitName("target") then
        return 1
    else
        return 0
    end
end

function CorpsePosition(coord)
    -- Assigns death coordinates
    local cX
    local cY
    if UnitIsGhost("player") then
        local map = C_Map.GetBestMapForUnit("player")
        if C_DeathInfo.GetCorpseMapPosition(map) ~= nil then
            cX, cY = C_DeathInfo.GetCorpseMapPosition(map):GetXY()
        end
    end
    if coord == "x" then
        if cX ~= nil then
            return cX
        else
            return 0
        end
        
    end
    if coord == "y" then
        if cY ~= nil then
            return cY
        else
            return 0
        end
    end
end

function GetEnemyStatus()
    local targStatus = UnitIsDead("target")
    if targStatus then
        return 1
    else
        return 0
    end
end

function TargetCanAttach()
    local t = UnitCanAttack("player", "target")
    if t then
        return 1
    else
        return 0
    end
end

function targetCombatStatus()
    local combatStatus = UnitAffectingCombat("target")
    -- if target is in combat, return 1 for bitmask
    if combatStatus then
        return 1
        -- if target is not in combat, return 0 for bitmask
    else return 0
    end
end

function deadOrAlive()
    local deathStatus = UnitIsGhost("player")
    if deathStatus then
        return 1
    else
        return 0
    end
end

function playerCombatStatus()
    local combatStatus = UnitAffectingCombat("player")
    -- if player is not in combat, convert nil to 0
    if combatStatus then
        return 1
    else
        return 0
    end
end

-- Returns the slot in which we have a fully degraded item
function isBroken()
    for i = 16, 17 do
        current, maximum = GetInventoryItemDurability(i)
		curdur = current/maximum
        if curdur < 0.05 then
            return 1
        end
    end
    return 0
end

function isInRange()
    local range = 0
    if IsActionInRange(1, "target") then range = 1 end -- Checks Root Range, slot 2
    return range
end

function getTargetUUID()
    local guid = UnitGUID("target")
    if guid == nil then
        return 0
    end
	
	guid = string.sub(guid,-17, -12)
	if tonumber(guid) ~= nil then return guid end;
	local ret = ""
    for i = 1, string.len(guid), 1 do
        local sub = string.sub(guid, i, i + 1)
        ret = ret .. string.format("%X", string.byte(sub))
    end

    local g = 0
    for i = 1, string.len(ret), 4 do
        g = g + tonumber(string.sub(ret, i, i + 4), 16)
    end
    for i = 1, string.len(ret), 3 do
        g = g + tonumber(string.sub(ret, i, i + 3), 16)
    end
    for i = 1, string.len(ret), 2 do
        g = g + tonumber(string.sub(ret, i, i + 2), 16)
    end
    for i = 1, string.len(ret), 1 do
        g = g + tonumber(string.sub(ret, i, i + 1), 16)
    end
    return g
end

loop:SetScript("OnUpdate",loop.onUpdate)