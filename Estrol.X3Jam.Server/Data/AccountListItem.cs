namespace Estrol.X3Jam.Server.Data {
    public class AccountListItem {
        /// <summary>
        /// Account number
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// Account Name
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Account Password (In plain text for some reason)
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Account Gender
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// Account IsAdmin
        /// </summary>
        public int IsAdmin { get; set; }
    }
}
