using System;

namespace Frends.AzureBlobStorage.DeleteBlob
{
    /// <summary>
    /// Extension-class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Method to convert enum to other enum.
        /// This method doesn't work as class helper method since it is used as extension for SnapshotDeleteOption.
        /// </summary>
        public static TEnum ConvertEnum<TEnum>(this Enum source)
        {
            return (TEnum) Enum.Parse(typeof(TEnum), source.ToString(), true);
        }
    }
}
