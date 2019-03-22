﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPromotionBenefitDetailsViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Catalog.Engine.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Framework.Pipelines;
    using CatalogConstants = Feature.Catalog.Engine.CatalogConstants;

    /// <inheritdoc />
    /// <summary>Defines the get promotion benefit details view pipeline block</summary>
    [PipelineDisplayName(CatalogConstants.Pipelines.Blocks.GetPromotionBenefitDetailsView)]
    public class GetPromotionBenefitDetailsViewBlock : BaseApplyCategoryAutocompleteForGetBlock
    {
        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetPromotionBenefitDetailsViewBlock(CommerceCommander commander)
          : base(commander)
        {
        }

        /// <summary>Gets the action name.</summary>
        /// <param name="context">The context.</param
        /// <returns>The action name.</returns>
        protected override string GetActionName(CommercePipelineExecutionContext context)
        {
            return context.GetPolicy<KnownPromotionsActionsPolicy>().EditBenefit;
        }

        /// <summary>Gets the entity view name.</summary>
        /// <param name="context">The context.</param
        /// <returns>The entity view name.</returns>
        protected override string GetEntityViewName(CommercePipelineExecutionContext context)
        {
            return context.GetPolicy<KnownPromotionsViewsPolicy>().BenefitDetails;
        }
    }
}