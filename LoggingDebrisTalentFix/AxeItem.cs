// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable
namespace Eco.Mods.TechTree
{
    using System;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Core.Utils.AtomicAction;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Plants;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Simulation;
    using Eco.Simulation.Types;
    using Eco.Stats;
    using Eco.World;
    using Eco.World.Blocks;

    [Category("Hidden"), Tag("Logging")]
    public partial class AxeItem
    {
        static IDynamicValue caloriesBurn                = new ConstantValue(0);
        static IDynamicValue damage                      = new ConstantValue(100);
        static IDynamicValue tier                        = new ConstantValue(0);
        static IDynamicValue skilledRepairCost           = new ConstantValue(1);
        static IDynamicValue debrisCaloriesBurnMultiplier = new TalentModifiedValue(typeof(AxeItem), typeof(LoggingCleanupCrewTalent), 3f); 

        public override Item            RepairItem            => Item.Get<StoneItem>();
        public override int             FullRepairAmount      => 1;
        public override IDynamicValue   CaloriesBurn          => caloriesBurn;
        public override IDynamicValue   Damage                => damage;
        public override IDynamicValue   Tier                  => tier;
        public override IDynamicValue   SkilledRepairCost     => skilledRepairCost;
        public override Type            ExperienceSkill       => typeof(LoggingSkill);
        public override LocString       LeftActionDescription => Localizer.DoStr("Chop");

        public override bool IsValidFor(Item item) => item is LogItem;

        // Highlight debris.
        public override bool ShouldHighlight(Type block) => Block.Is<TreeDebris>(block);

        // Delete debris or pass it.
        public override InteractResult OnActLeft(InteractionContext context)
        {
            // Try delete tree debris with reduced XP multiplier.
            if (context.HasBlock && context.Block.Get<TreeDebris>() is TreeDebris treeDebris)
                // Create game action pack, compose and try to perform.
                using (var pack = new GameActionPack()) 
                {
                    // Add debris items to inventory. 
                    foreach (var x in ((TreeSpecies)EcoSim.GetSpecies(treeDebris.Species)).DebrisResources)
                        pack.AddToInventory(context.Player?.User.Inventory, Item.Get(x.Key), x.Value.RandInt, context.Player?.User);

                    // Create multiblock context with reduced XP multiplier for cleaning debris. 
                    var multiblockContext = this.CreateMultiblockContext(context);
                    multiblockContext.ActionDescription    = Localizer.DoStr("clean up tree debris");
                    multiblockContext.ExperiencePerAction *= 0.1f;
                    
                    // Check if the player has the talent
                    if((int)debrisCaloriesBurnMultiplier.GetCurrentValue(context.Player?.User.DynamicValueContext) == 1)
                    {
                        multiblockContext.CaloriesPerAction = 0;
                    }

                    // Add block deletion to the pack and try to perform it.
                    pack.DeleteBlock(multiblockContext);
                    return (InteractResult)pack.TryPerform();
                }

            // Try interact with a world object.
            if (context.Target is WorldObject) return this.BasicToolOnWorldObjectCheck(context);

            // Fallback (try to damage target).
            return base.OnActLeft(context);
        }

    }
}
