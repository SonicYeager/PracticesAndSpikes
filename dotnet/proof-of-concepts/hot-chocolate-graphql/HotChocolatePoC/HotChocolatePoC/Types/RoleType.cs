namespace HotChocolatePoC.Types
{
    /// <summary>
    /// Represents the roles for users.
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Represents the Purchaser.
        /// </summary>
        Purchaser = 0,

        /// <summary>
        /// Represents the Supplier.
        /// </summary>
        Supplier = 1,

        /// <summary>
        /// Represents the Container Team.
        /// </summary>
        ContainerTeam = 3,

        /// <summary>
        /// Represents the Freight Forwarder.
        /// </summary>
        FreightForwarder = 5,
    }
}