﻿using System;
using System.Collections.Generic;
using System.Linq;
using Prism.API.Behaviours;
using Prism.Mods;
using Prism.Mods.DefHandlers;
using Terraria;
using Terraria.ID;

namespace Prism.API.Defs
{
    public class NpcRef : EntityRef<NpcDef, NpcBehaviour, NPC>
    {
        public NpcRef(int resourceId)
            : base(Handler.NpcDef.DefsByType.ContainsKey(resourceId) ? Handler.NpcDef.DefsByType[resourceId].InternalName : String.Empty)
        {
            if (resourceId >= NPCID.Count)
                throw new ArgumentOutOfRangeException("resourceId", "The resourceId must be a vanilla NPC type or netID.");
        }
        public NpcRef(string resourceName, string modName = null)
            : base(resourceName, modName)
        {

        }

        public override NpcDef Resolve()
        {
            if (IsVanillaRef)
            {
                if (!Handler.NpcDef.VanillaDefsByName.ContainsKey(ResourceName))
                    throw new InvalidOperationException("Vanilla NPC reference '" + ResourceName + "' is not found.");

                return Handler.NpcDef.VanillaDefsByName[ResourceName];
            }

            if (!ModData.Mods.Keys.Any(mi => mi.InternalName == ModName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the mod is not loaded.");
            if (!ModData.Mods.First(mi => mi.Key.InternalName == ModName).Value.ItemDefs.ContainsKey(ResourceName))
                throw new InvalidOperationException("NPC reference '" + ResourceName + "' in mod '" + ModName + "' could not be resolved because the NPC is not loaded.");

            return NpcDef.ByName[ResourceName, ModName];
        }
    }
}
