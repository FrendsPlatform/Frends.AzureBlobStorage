using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// List blobs in a flat listing structure or hierarchically.
    /// Hierarchical listing returns container's blobs and subdirectories names such as file.txt or directoryname/. 
    /// Flat listing does the same as hierarchical listing but also returns blobs in those subdirectories such as directoryname/file.txt.
    /// </summary>
    /// <example>ListingStructure.Flat</example>
    [DefaultValue(ListingStructure.Flat)]
    public ListingStructure ListingStructure { get; set; }

    /// <summary>
    /// Specify a prefix to return blobs whose names begin with that character or string.
    /// </summary>
    /// <example>test</example>
    public string Prefix { get; set; }

	/// <summary>
	/// The type of query to be executed.
	/// </summary>
	[DefaultValue(QueryType.Default)]
	[UIHint(nameof(ListingStructure), "", ListingStructure.Flat)]
	public QueryType QueryType { get; set; } = QueryType.Default;

	/// <summary>
	/// The query string used to filter or search for tags.
	/// </summary>
	/// <example>
	/// <![CDATA[createdUtc <= '2025-01-01']]>
	/// </example>
	[UIHint(nameof(QueryType), "", QueryType.Tags)]
	public string TagQuery { get; set; } = "";
}
