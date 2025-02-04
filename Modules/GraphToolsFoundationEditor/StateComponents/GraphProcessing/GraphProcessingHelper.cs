// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.GraphToolsFoundation.Editor
{
    /// <summary>
    /// Graph processing options.
    /// </summary>
    enum RequestGraphProcessingOptions
    {
        /// <summary>
        /// Process the graph.
        /// </summary>
        Default,

        /// <summary>
        /// Save the graph and process it.
        /// </summary>
        SaveGraph,
    }

    /// <summary>
    /// Helper class for graph processing.
    /// </summary>
    static class GraphProcessingHelper
    {
        /// <summary>
        /// Processes the graph using the graph processors provided by <see cref="Stencil.GetGraphProcessorContainer"/>.
        /// </summary>
        /// <param name="graphModel">The graph to process.</param>
        /// <param name="changeset">A description of what changed in the graph. If null, the method assumes everything changed.</param>
        /// <param name="options">Graph processing options.</param>
        /// <returns>The results of the graph processing.</returns>
        public static IReadOnlyList<GraphProcessingResult> ProcessGraph(
            GraphModel graphModel,
            GraphModelStateComponent.Changeset changeset,
            RequestGraphProcessingOptions options)
        {
            var stencil = (Stencil)graphModel?.Stencil;
            if (stencil == null)
                return null;

            GraphChangeDescription changes = null;
            if (changeset != null)
            {
                changes = new GraphChangeDescription(changeset.NewModels, changeset.ChangedModelsAndHints, changeset.DeletedModels);
            }

            if (options == RequestGraphProcessingOptions.SaveGraph && graphModel.Asset != null)
                graphModel.Asset.Save();

            return stencil.GetGraphProcessorContainer().ProcessGraph(graphModel, changes);
        }

        /// <summary>
        /// Converts the errors generated by the processing of the graph to instances of <see cref="GraphProcessingErrorModel"/>.
        /// </summary>
        /// <param name="stencil">The stencil.</param>
        /// <param name="results">The graph processing results used as the source of errors to convert.</param>
        /// <returns>The converted errors.</returns>
        public static IEnumerable<GraphProcessingErrorModel> GetErrors(Stencil stencil, IReadOnlyList<GraphProcessingResult> results)
        {
            if (results != null && results.Any())
            {
                return results.SelectMany(r => r.Errors.Select(stencil.CreateProcessingErrorModel).Where(m => m != null));
            }

            return Enumerable.Empty<GraphProcessingErrorModel>();
        }
    }
}
