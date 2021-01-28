namespace Eco.Mods.TechTree
{
// [DoNotLocalize]
using System;
using System.Collections.Generic;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Systems.TextLinks;
using Eco.Gameplay.Systems.Tooltip;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

[Serialized]
[LocDisplayName("Conveyor Belt")]
[Weight(100)]
public partial class ConveyorBeltItem : WorldObjectItem<ConveyorBeltObject>
{
    public override LocString DisplayDescription { get { return Localizer.DoStr("Convey things with belts"); } }

    static ConveyorBeltItem() { }
    }

    [RequiresSkill(typeof(IndustrySkill), 1)]
    public partial class ConveyorBeltRecipe : RecipeFamily
    {
        public ConveyorBeltRecipe()
        {
            this.Recipes = new List<Recipe>
            {
                new Recipe(
                        "ConveyorBelt",
                        Localizer.DoStr("Conveyor Belt"),
                        new IngredientElement[]
                        {
                            new IngredientElement(typeof(IronGearItem), 1, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                            new IngredientElement(typeof(CorrugatedSteelItem), 1, typeof(IndustrySkill), typeof(IndustryLavishResourcesTalent)),
                        },
                        new CraftingElement[] { new CraftingElement<ConveyorBeltItem>() }
                    )
            };

            this.ExperienceOnCraft = 5;
            this.LaborInCalories = CreateLaborInCaloriesValue(50);
            this.CraftMinutes = CreateCraftTimeValue(typeof(ConveyorBeltRecipe), 1, typeof(IndustrySkill), typeof(IndustryFocusedSpeedTalent), typeof(IndustryParallelSpeedTalent));
            this.Initialize(Localizer.DoStr("Conveyor Belt"), typeof(ConveyorBeltRecipe));

            CraftingComponent.AddRecipe(typeof(AssemblyLineObject), this);
        }
    }
}