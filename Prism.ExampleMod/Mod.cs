﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Prism.API;
using Prism.API.Defs;
using Terraria;
using Terraria.ID;

namespace Prism.ExampleMod
{
    public class Mod : ModDef
    {
        public static KeyboardState prevKeyState = new KeyboardState();
        public static int meowmaritusHappyFunCount = -1;
        public static int meowmaritusHappyFunTimeBytes = 0x14018E;

        protected override Dictionary<string, ItemDef      > GetItemDefs      ()
        {
            return new Dictionary<string, ItemDef>
            {
                // Pizza done with JSON method using an external resource
                { "Pizza", new ItemDef("Pizza", GetResource<JsonData>("Resources/Items/Pizza.json"),
                    () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png")) },
                // Ant done with JSON method using an embedded resource
                { "Ant", new ItemDef("Ant", GetEmbeddedResource<JsonData>("Resources/Items/Ant.json"),
                    () => GetEmbeddedResource<Texture2D>("Resources/Textures/Items/Ant.png")) },
                { "Pizzant", new ItemDef("Pizzant", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizzant.png"))
                {   Description = new ItemDescription("The chaotic forces of italian spices and insects and bread.", expert: true),
                    DamageType = ItemDamageType.Melee,
                    AutoReuse = true,
                    UseTime = 12,
                    ReuseDelay = 0,
                    UseAnimation = 8,
                    MaxStack = 1,
                    Rarity = ItemRarity.Yellow,
                    UseSound = 1,
                    Damage = 80,
                    Knockback = 4f,
                    Width = 30,
                    Height = 30,
                    TurnPlayerOnUse = false,
                    UseStyle = ItemUseStyle.Stab,
                    HoldStyle = ItemHoldStyle.Default,
                    Value = new CoinValue(1, 34, 1, 67),
                    Scale = 1.1f
                } },
                { "Pizzantzioli", new ItemDef("Pizzantzioli", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizzantzioli.png"))
                {   Description = new ItemDescription("The forces of ants and pizza come together as one.", "The name is Italian for 'KICKING ASS'! YEAH! BROFISSSSST!!1!", expert: true),
                    DamageType = ItemDamageType.Melee,
                    AutoReuse = true,
                    UseTime = 20,
                    ReuseDelay = 0,
                    UseAnimation = 16,
                    MaxStack = 1,
                    Rarity = ItemRarity.Cyan,
                    UseSound = 1,
                    Damage = 150,
                    Knockback = 10f,
                    Width = 30,
                    Height = 30,
                    TurnPlayerOnUse = false,
                    UseStyle = ItemUseStyle.Swing,
                    HoldStyle = ItemHoldStyle.Default,
                    Value = new CoinValue(2, 51, 3, 9),
                    Scale = 1.1f
                } }
            };
        }
        protected override Dictionary<string, NpcDef       > GetNpcDefs       ()
        {
            return new Dictionary<string, NpcDef>
            {
                { "PizzaNPC", new NpcDef("Possessed Pizza", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {   MaxLife = 80,
                    FrameCount = 1,
                    Damage = 5,
                    Width = 64,
                    Height = 64,
                    Alpha = 0,
                    Scale = 1.0f,
                    IgnoreTileCollision = true,
                    Colour = Color.White,
                    Value = new NpcValue((CoinValue)0),
                    AiStyle = NpcAiStyle.FlyingWeapon                
                } },
                { "PizzaBoss", new NpcDef("Pizza God", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"), () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                {   
                    MaxLife = 1000,
                    FrameCount = 1,
                    Damage = 5,
                    Width = 64,
                    Height = 64,
                    Alpha = 0,
                    Scale = 4.0f,
                    IgnoreTileCollision = true,
                    Colour = Color.White,
                    Value = new NpcValue((CoinValue)0),
                    AiStyle = NpcAiStyle.EyeOfCthulhu,
                    IsBoss = true,
                    IsSummonableBoss = true                   
                } }
            };
        }
        protected override Dictionary<string, ProjectileDef> GetProjectileDefs()
        {
            return new Dictionary<string, ProjectileDef>
            {
                { "PizzaProjectile", new ProjectileDef("Flying Pizza!", null, () => GetResource<Texture2D>("Resources/Textures/Items/Pizza.png"))
                }
            };
        }

        protected override IEnumerable<RecipeDef> GetRecipeDefs()
        {
            return new[]
            {
                new RecipeDef(
                    new ItemRef("Pizza", Info.InternalName), 8,
                    new RecipeItems
                    {
                        { new ItemRef(ItemID.Gel), 30 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Ant", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza", Info.InternalName),  1 },
                        { new ItemRef(ItemID.Gel                ), 20 }
                    }
                ),
                new RecipeDef(
                    new ItemRef("Pizzant", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza", Info.InternalName), 1 },
                        { new ItemRef("Ant"  , Info.InternalName), 1 },
                        { new ItemRef(ItemID.Gel                ), 4 }
                    },
                    new[] { TileID.TinkerersWorkbench }
                ),
                new RecipeDef(
                    new ItemRef("Pizzantzioli", Info.InternalName), 1,
                    new RecipeItems
                    {
                        { new ItemRef("Pizza"  , Info.InternalName), 3 },
                        { new ItemRef("Pizzant", Info.InternalName), 1 },
                        { new ItemRef(ItemID.Gel                  ), 4 }
                    },
                    new[] { TileID.Dirt }
                )
            };
        }

        //TODO: put this somewhere in Prism
        public static bool GetKey(Keys k)
        {
            return Main.keyState.IsKeyDown(k);
        }
        public static bool GetKey(Keys k, KeyState onEnterKeyState)
        {
            return onEnterKeyState == Main.keyState[k] && Main.keyState[k] != prevKeyState[k];
        }

        public static Vector2 GetRandomPositionOnScreen()
        {
            return new Vector2(Main.screenPosition.X + (float)Main.rand.NextDouble() * Main.screenWidth, Main.screenPosition.Y + (float)Main.rand.NextDouble() * Main.screenHeight);
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu || !Main.hasFocus || Main.chatMode)
                return;

            var p = Main.player[Main.myPlayer];

            #region CHEATERRRRRRRRRR
            if (GetKey(Keys.G))
            {
                Point me = Main.player[Main.myPlayer].Center.ToPoint();
                Item.NewItem(me.X, me.Y, 1, 1, ItemID.Gel, 10, false, 0, true, false);
            }
            #endregion

            #region imma tell on u O.o
            if (GetKey(Keys.I, KeyState.Down))
            {
                Point me = Main.player[Main.myPlayer].Center.ToPoint();
                Item.NewItem(me.X, me.Y, 1, 1, ItemID.Gel, 10, false, 0, true, false);
            }
            #endregion

            #region I dare you to press L
            if (GetKey(Keys.L, KeyState.Down))
            {
                meowmaritusHappyFunCount = (byte)(meowmaritusHappyFunTimeBytes >> 16);
            }

            if (!Main.player[Main.myPlayer].dead)
            {
                if (meowmaritusHappyFunCount-- > 0)
                {
                    NPC.defaultMaxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);
                    NPC.maxSpawns *= (byte)(meowmaritusHappyFunTimeBytes >> 16);

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!Main.npc[i].active)
                        {
                            Main.npc[i] = new NPC();
                            Main.npc[i].SetDefaults((short)meowmaritusHappyFunTimeBytes, -1);
                            Main.npc[i].active = true;
                            Main.npc[i].timeLeft = NPC.activeTime;
                            Main.npc[i].position = GetRandomPositionOnScreen();
                            Main.npc[i].ai[1] = 59f;
                            Main.PlaySound(29, (int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, 92);

                            break;
                        }                           
                    }
                }
            }
            #endregion

            #region spawn custom npcs
            if (GetKey(Keys.N, KeyState.Down))
            {
                var pt = GetRandomPositionOnScreen().ToPoint();

                NPC.NewNPC(pt.X, pt.Y, NpcDef.ByName["PizzaNPC", Info.InternalName].Type);
            }

            if (GetKey(Keys.B, KeyState.Down))
            {
                NPC.SpawnOnPlayer(Main.myPlayer, NpcDef.ByName["PizzaBoss", Info.InternalName].Type);
            }
            #endregion

            prevKeyState = Main.keyState;
        }

        public override void UpdateMusic()
        {
            if (Main.gameMenu || !Main.hasFocus)
                return;

            Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            int pizzaBossType = NpcDef.ByName["PizzaBoss", Info.InternalName].Type;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == pizzaBossType)
                {
                    Rectangle npcRect = new Rectangle((int)(Main.npc[i].position.X + (float)(Main.npc[i].width / 2)) - 5000, (int)(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2)) - 5000, 10000, 10000);
                    if (screen.Intersects(npcRect))
                    {
                        Main.curMusic = 25;
                    }
                }
            }
        }
    }
}
