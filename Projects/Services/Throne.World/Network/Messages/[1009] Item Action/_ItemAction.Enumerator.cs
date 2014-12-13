﻿namespace Throne.World.Network.Messages
{
    public enum ItemActionType
    {
        None,
        Buy,
        Sell,
        Remove,
        Use,
        Equip,
        Unequip,
        Split,
        Combine,
        QueryDepository,
        PutDepository,
        TakeDepository,
        TakeDepositoryFunds,
        DropMoney,
        Repair = 14,
        VIPRepair = 15,
        Ident,
        Durability,
        DropEquipment,
        DragonballImprove = 19,
        MeteorCraft = 20,
        BoothQuery = 21,
        BoothAdd = 22,
        BoothRemove = 23,
        BoothBuy = 24,
        UpdateDurability = 25,
        Fireworks = 26,
        Ping = 27,
        Enchant = 28,
        BoothAddEMoney = 29,
        DetainedItemRedemption = 32,
        DetainedItemClaim = 33,
        SocketTalismanWithItem = 35,
        SocketTalismanWithCPs = 36,
        DropItem = 37,
        _DropMoney = 38,
        GemCompose = 39,
        Bless = 40,
        ActivateAccessory = 41,
        Socketeer = 43,
        MainGearSwitch = 44,
        AlternateGearSwitch = 45,
        Gear = 46,
        MergeItemStack = 48,
        SplitItemStack = 49,
        ClaimableItem = 50,
        MergeTortoiseGem = 51,
        ItemTooltip = 52,
        DelevelItem = 54,
        BuyFromForge = 55
    }
}