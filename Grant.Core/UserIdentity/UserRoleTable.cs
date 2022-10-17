﻿using System.Collections.Generic;
using Grant.Core.DbContext;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that represents the AspNetUserRoles table in the PostgreSQL Database.
    /// </summary>
    public class UserRolesTable
    {
        private PostgreSQLDatabase _database;

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a list of user's roles.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            List<string> roles = new List<string>();
            //TODO: This probably does not work, and may need testing.
            string commandText = "SELECT \"AspNetRoles\".\"Name\" FROM dbo.\"AspNetUsers\", dbo.\"AspNetRoles\", dbo.\"AspNetUserRoles\" ";
                   commandText += "WHERE dbo.\"AspNetUsers\".\"Id\" = dbo.\"AspNetUserRoles\".\"UserId\" AND dbo.\"AspNetUserRoles\".\"RoleId\" = dbo.\"AspNetRoles\".\"Id\";";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                roles.Add(row["Name"]);
            }

            return roles;
        }

        /// <summary>
        /// Deletes all roles from a user in the AspNetUserRoles table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM \"AspNetRoles\" WHERE \"UserId\" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role record for a user in the UserRoles table.
        /// </summary>
        /// <param name="user">The User.</param>
        /// <param name="roleId">The Role's id.</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, string roleId)
        {
            string commandText = "INSERT INTO \"AspNetUserRoles\" (\"UserId\", \"RoleId\") VALUES (@userId, @roleId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("roleId", roleId);

            return _database.Execute(commandText, parameters);
        }
    }
}
