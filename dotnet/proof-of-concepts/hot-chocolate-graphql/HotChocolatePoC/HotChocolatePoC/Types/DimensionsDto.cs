using System.ComponentModel.DataAnnotations;

namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents Dimensions.
    /// </summary>
    public class DimensionsDto
    {
        /// <summary>
        /// The height in cm.
        /// </summary>
        [Range(0.1, 10_000)]
        public double Height { get; set; }

        /// <summary>
        /// The width in cm.
        /// </summary>
        [Range(0.1, 10_000)]
        public double Width { get; set; }

        /// <summary>
        /// The length in cm.
        /// </summary>
        [Range(0.1, 10_000)]
        public double Length { get; set; }
    }
}